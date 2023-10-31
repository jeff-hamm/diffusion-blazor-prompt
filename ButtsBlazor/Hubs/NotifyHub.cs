using ButtsBlazor.Api.Services;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace ButtsBlazor.Hubs;

public class NotifyHub(PromptQueue queue, ILogger<NotifyHub> logger, FileService fileService) : Hub
{
    public const string ListenersGroup = nameof(ListenersGroup);
    public const string ProcessorsGroup = nameof(ProcessorsGroup);

    public async Task StartListening()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, ListenersGroup);
//        return new PromptState((await fileService.GetLatestUploads(1))?.FirstOrDefault());
    }

    public Task StopListening()
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, ListenersGroup);
    }

    public async Task StartProcessing()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, ProcessorsGroup);
    }

    public Task StopProcessing()
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, ProcessorsGroup);
    }

    public Task StatusMessage(string message) => Clients.StatusMessage(message);

    public async Task ProcessComplete(int id, string base64Image)
    {
        // write image to storage
        var completed = await queue.ProcessComplete(id, base64Image);
        if (completed != null)
            await Clients.ProcessComplete(completed);
        else
            logger.LogWarning("Could not send completed message for {id}, no prompts found",id);
    }

    public async Task<int> Prompt(string? prompt, string? negativePrompt, int? controlImageId)
    {
        var args = await queue.Enqueue(prompt, negativePrompt, controlImageId);
        await Clients.Group(ProcessorsGroup).SendAsync("NewPrompt", args.RowId);
        return args.RowId;
    }

    public Task<PromptArgs?> ProcessNext() => queue.ProcessNext();
}