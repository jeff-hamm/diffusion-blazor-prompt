using System.Collections.Concurrent;
using ButtsBlazor.Client.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Services;

public class PromptQueue(ILogger<PromptQueue> logger, IOptions<PromptOptions> config, FileService fileService)
{
    private ConcurrentQueue<PromptArgs> Pending { get; } = new();
    private ConcurrentDictionary<Guid, PromptArgs> Processing { get; } = new();
    private ConcurrentQueue<PromptArgs> History { get; } = new();

    public void Enqueue(PromptArgs buttPromptArgs)
    {
        buttPromptArgs.Enqueued = DateTimeOffset.UtcNow;
        Pending.Enqueue(buttPromptArgs);
    }

    public async Task<PromptArgs?> ProcessNext()
    {
        if (!Pending.TryDequeue(out var args))
            return null;
        args.ProcessingStart = DateTimeOffset.UtcNow;
        Processing.AddOrUpdate(args.Id, args, (_, __) => args);
        if (!String.IsNullOrEmpty(args.ControlFilePath)) 
            args.ControlFile =  await fileService.ReadAsBase64Contents(args.ControlFilePath);
        return args;
    }

    public async Task<PromptArgs?> ProcessComplete(Guid id, string base64File)
    {
        if (!Processing.TryRemove(id, out var processed))
        {
            logger.LogWarning("Dequeue error, could not find {id} in processing queue", id);
            return null;
        }

        processed.ImagePath = await fileService.SaveOutputFile(processed.Prompt, base64File, ".png");
        processed.ProcessingCompleted = DateTimeOffset.UtcNow;
        // Clear this out for garbage collection
        processed.ControlFile = null;
        History.Enqueue(processed);
        if (History.Count > config.Value.HistoryLength)
            History.TryDequeue(out var _);
        return processed;
    }
}