using System.Collections.Concurrent;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Api.Services;

public class PromptQueue(ILogger<PromptQueue> logger, FileService fileService, IDbContextFactory<ButtsDbContext> contextFactory)
{
    private ConcurrentQueue<PromptArgs> Pending { get; } = new();
    private ConcurrentDictionary<Guid, PromptArgs> Processing { get; } = new();

    public async Task<PromptArgs> Enqueue(string? prompt, string? negativePrompt, string? controlImageHash)
    {
        var args = await CreatePromptArgs(prompt, negativePrompt, controlImageHash);
        Pending.Enqueue(args);
        return args;
    }

    private async Task<PromptArgs> CreatePromptArgs(string? prompt, string? negativePrompt, string? controlImageHash)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var controlFile = controlImageHash != null
            ? await db.Images.FirstOrDefaultAsync(img => img.Base64Hash == controlImageHash)
            : null;
        var args = new PromptArgs()
        {
            Id = Guid.NewGuid(),
            Prompt = prompt ?? "",
            Negative = negativePrompt,
            ControlFilePath = controlFile?.Path
        };
        db.PromptArgs.Add(args);
        db.Prompts.Add(new PromptEntity()
        {
            Id = args.Id,
            Args = args,
            Enqueued = DateTimeOffset.UtcNow,
            ControlImageId = controlFile?.Id,
            ControlImage = controlFile
        });
        await db.SaveChangesAsync();
        return args;
    }

    public async Task<PromptArgs?> ProcessNext()
    {
        if (!Pending.TryDequeue(out var args))
            return null;
        await StartProcessing(contextFactory, args);
        Processing.AddOrUpdate(args.Id, args, (_, __) => args);
        if (!String.IsNullOrEmpty(args.ControlFilePath)) 
            args.ControlFile =  await fileService.ReadAsBase64Contents(args.ControlFilePath);
        return args;
    }

    private static async Task StartProcessing(IDbContextFactory<ButtsDbContext> contextFactory, PromptArgs args)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var prompt = await db.Prompts.FindAsync(args.Id);
        if (prompt != null)
        {
            prompt.ProcessingStart = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    public async Task<PromptEntity?> ProcessComplete(Guid id, string base64File)
    {
        if (!Processing.TryRemove(id, out var processed))
        {
            logger.LogWarning("Dequeue error, could not find {id} in processing queue", id);
            return null;
        }
        await using var db = await contextFactory.CreateDbContextAsync();
        var prompt = await db.Prompts.FindAsync(id);
        if (prompt == null) return null;
        var outputFile = await fileService.SaveOutputFile(processed.Prompt, base64File, ".png");
        prompt.OutputImage = new ImageEntity()
        {
            Id = Guid.NewGuid(),
            Path = outputFile.Uri,
            Base64Hash = outputFile.Base64Hash,
            Type = ImageType.Output
        };
        prompt.ProcessingCompleted = DateTimeOffset.UtcNow;
        // Clear this out for garbage collection
        processed.ControlFile = null;
        await db.SaveChangesAsync();
        return prompt;
    }
}