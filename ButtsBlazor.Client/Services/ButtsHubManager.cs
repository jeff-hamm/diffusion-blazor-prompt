using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Utils;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using PubSub;

namespace ButtsBlazor.Client.Services;

public class ButtsHubManager(Hub pubsub, PromptOptions options, IWebAssemblyHostEnvironment hostEnvironment) : IAsyncDisposable
{

    private HubConnection? hubConnection;
    public async Task Initialize()
    {
        if (hubConnection == null)
        {
            hubConnection = new HubConnectionBuilder()
                .ConfigureLogging(lb => lb.SetMinimumLevel(options.LogLevel))
                .WithUrl(hostEnvironment.BaseAddress + options.NotifyHubPath)
                .WithAutomaticReconnect()
                .Build();
            hubConnection.Reconnecting += OnReconnecting;
            hubConnection.Reconnected += OnReconnected;
            hubConnection.On<ImageEntity>("NewControlImage", OnImageReceived);
        }

        if (hubConnection.State == HubConnectionState.Disconnected)
        {
            await hubConnection.StartAsync();
            LastState = await hubConnection.InvokeAsync<PromptState>("StartListening");
        }
    }

    public PromptState? LastState { get; set; }

    private Task OnReconnected(string? s)
    {
        Console.WriteLine("Reconnected" + s);
        return Task.CompletedTask;
    }

    private Task OnReconnecting(Exception? s)
    {
        Console.WriteLine("Reconnecting" + s);
        return Task.CompletedTask;
    }
    public ImageEntity? LastUpload { get; set; }
    private async Task OnImageReceived(ImageEntity image)
    {
        LastUpload = image;
        await pubsub.PublishAsync(image);
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null) await hubConnection.DisposeAsync();
    }
}