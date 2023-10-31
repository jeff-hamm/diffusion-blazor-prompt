using System.Collections.Concurrent;
using System.IO;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Utils;
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
    private ConcurrentDictionary<int, PromptArgs> Processing { get; } = new();

    public async Task<PromptArgs> Enqueue(string? prompt, string? negativePrompt, int? controlImageId)
    {
        var args = await CreatePromptArgs(prompt, negativePrompt, controlImageId);
        Pending.Enqueue(args);
        return args;
    }

    private async Task<PromptArgs> CreatePromptArgs(string? prompt, string? negativePrompt, int? controlImageId)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var controlFile = controlImageId.HasValue
            ? await db.Images.FindAsync(controlImageId.Value) : null;
        var args = new PromptArgs()
        {
            Prompt = prompt ?? "",
            Negative = negativePrompt,
            ControlFilePath = controlFile?.Path
        };
        db.PromptArgs.Add(args);
        db.Prompts.Add(new PromptEntity()
        {
            RowId = args.RowId,
            Args = args,
            Enqueued = DateTime.UtcNow,
            ControlImageId = controlFile?.RowId,
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
        Processing.AddOrUpdate(args.RowId, args, (_, __) => args);
        if (args.ControlFilePath?.FilePath != null)
            args.ControlFile = await args.ControlFilePath.Value.FilePath.TryReadAsBase64Contents();

        return args;
    }

    private static async Task StartProcessing(IDbContextFactory<ButtsDbContext> contextFactory, PromptArgs args)
    {
        await using var db = await contextFactory.CreateDbContextAsync();
        var prompt = await db.Prompts.FindAsync(args.RowId);
        if (prompt != null)
        {
            prompt.ProcessingStart = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    public async Task<PromptEntity?> ProcessComplete(int id, string base64File)
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
        var outputImg = new ImageEntity()
        {
            Path = outputFile.Uri,
            Base64Hash = outputFile.Base64Hash,
            Type = ImageType.Output
        };
        db.Add(outputImg);
        prompt.OutputImageId = outputImg.RowId; 
        prompt.ProcessingCompleted = DateTime.UtcNow;
        // Clear this out for garbage collection
        processed.ControlFile = null;
        await db.SaveChangesAsync();
        return prompt;
    }
}