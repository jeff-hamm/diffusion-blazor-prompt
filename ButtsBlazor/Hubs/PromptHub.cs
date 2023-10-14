using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Telerik.DataSource;

namespace ButtsBlazor.Hubs;

public static class PromptHubMessageExtensions
{

    public static IClientProxy Listeners(this IHubClients<IClientProxy> @this) =>
        @this.Groups(PromptHub.ListenersGroup);

    public static Task StatusMessage(this IHubClients<IClientProxy> @this, string message) => 
        @this.Listeners().SendAsync("StatusMessage", message);
    public static Task NewControlImage(this IHubClients<IClientProxy> @this, HashedImage image) =>
        @this.Listeners().SendAsync("NewControlImage", image);

    public static Task ProcessComplete(this IHubClients<IClientProxy> @this, PromptEntity completed) => 
        @this.Listeners().SendAsync("ProcessComplete", completed.Id, completed.OutputImage?.Path);

    public static IClientProxy Processors(this IHubClients<IClientProxy> @this) =>
        @this.Groups(PromptHub.ProcessorsGroup);

}


public class PromptHub(PromptQueue queue, ILogger<PromptHub> logger, FileService fileService) : Hub
{
    public const string ListenersGroup = nameof(ListenersGroup);
    public const string ProcessorsGroup = nameof(ProcessorsGroup);

    public async Task<PromptState> StartListening()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, ListenersGroup);
        return new PromptState(fileService.GetLatestUpload());
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

    public async Task ProcessComplete(Guid id, string base64Image)
    {
        // write image to storage
        var completed = await queue.ProcessComplete(id, base64Image);
        if (completed != null)
            await Clients.ProcessComplete(completed);
        else
            logger.LogWarning("Could not send completed message for {id}, no prompts found",id);
    }

    public async Task<Guid> Prompt(string? prompt, string? negativePrompt, string? controlImageHash)
    {
        var args = await queue.Enqueue(prompt, negativePrompt, controlImageHash);
        await Clients.Group(ProcessorsGroup).SendAsync("NewPrompt", args.Id);
        return args.Id;
    }

    public Task<PromptArgs?> ProcessNext() => queue.ProcessNext();
}