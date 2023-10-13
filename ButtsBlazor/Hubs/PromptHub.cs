using ButtsBlazor.Client.Utils;
using ButtsBlazor.Invokable;
using ButtsBlazor.Services;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
namespace ButtsBlazor.Hubs;

public class PromptHub(PromptQueue queue, ILogger<PromptHub> logger, FileService fileService) : Hub
{
    public const string ListenersGroup = nameof(ListenersGroup);
    public const string ProcessorsGroup = nameof(ProcessorsGroup);
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public Task StartListening()
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, ListenersGroup);
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

    public Task StatusMessage(string message)
    {
        return Clients.Group(ListenersGroup).SendAsync("StatusMessage", message);
    }

    public async Task<Guid> Prompt(string? prompt, string? negativePrompt, string? controlImageHash)
    {
        var id = Guid.NewGuid();
        queue.Enqueue(new PromptArgs(id, prompt??"", negativePrompt,  
            String.IsNullOrEmpty(controlImageHash) ? "" :fileService.TryFindUploadFile(controlImageHash))); 
        await Clients.Group(ProcessorsGroup).SendAsync("NewPrompt", id);
        return id;
    }

    public Task<PromptArgs?> ProcessNext() => queue.ProcessNext();

    public async Task ProcessComplete(Guid id, string base64Image)
    {
        // write image to storage
        var completed = await queue.ProcessComplete(id, base64Image);
        await Clients.Group(ListenersGroup).SendAsync("ProcessComplete", id, completed?.ImagePath);
    }
}