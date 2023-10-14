using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Api.Utils;

namespace ButtsBlazor.Client.Pages;

partial class Prompt : ComponentBase
{
    private HubConnection? hubConnection;
    private string? imageUrl;
    private string? message;
    private string? promptInput;
    private string? negativePrompt;
    readonly CancellationTokenSource disposalTokenSource = new();
    private List<string> images = new() {};

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .ConfigureLogging(lb => lb.SetMinimumLevel(LogLevel.Debug))
            .WithUrl(Navigation.ToAbsoluteUri("/prompt"))
            .WithAutomaticReconnect()
            .Build();
        hubConnection.Reconnecting += OnReconnecting;
        hubConnection.Reconnected += OnReconnected;
        hubConnection.On<string>("StatusMessage", OnStatusMessageReceived);
        hubConnection.On<HashedImage>("NewControlImage", OnNewControlImageReceived);
        hubConnection.On<Guid, string>("ProcessComplete", OnProcessCompleteMessage);
        message = "Connecting...";
        StateHasChanged();
        await hubConnection.StartAsync();
        promptState = await hubConnection.InvokeAsync<PromptState>("StartListening");
        controlImage = promptState?.ControlImage;
        message = "Connected.";
        StateHasChanged();
    }

    private async Task OnReconnected(string? s)
    {
        this.message = "Reconnected";
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnReconnecting(Exception? s)
    {
        this.message = "Reconnecting";
        await InvokeAsync(StateHasChanged);
    }

    private void OnNewControlImageReceived(HashedImage image)
    {
        this.controlImage = image;
        StateHasChanged();
    }

    private void OnProcessCompleteMessage(Guid id, string url)
    {
        this.processedId = id;
        this.message = $"Completed: {id}";
        this.imageUrl = url + "?cache=" + DateTime.Now.Ticks;
        images.Add(this.imageUrl);
        StateHasChanged();
    }

    private void OnStatusMessageReceived(string status)
    {
        status = status.Trim();
        if (!String.IsNullOrEmpty(status))
        {
            this.message = status;
            StateHasChanged();
        }
    }


    // async Task ReceiveLoop()
    // {
    //     var buffer = new ArraySegment<byte>(new byte[1024]);
    //     while (!disposalTokenSource.IsCancellationRequested)
    //     {
    //         // Note that the received block might only be part of a larger message. If this applies in your scenario,
    //         // check the received.EndOfMessage and consider buffering the blocks until that property is true.
    //         // Or use a higher-level library such as SignalR.
    //             var received = await webSocket.ReceiveAsync(buffer, disposalTokenSource.Token);
    //             message = Encoding.UTF8.GetString(buffer.Array, 0, received.Count);
    //             StateHasChanged();
    //     }
    // }

    private Guid? promptId;
    private Guid? processedId;
    private PromptState? promptState;
    private HashedImage? controlImage;

    public async Task Send()
    {

        if (hubConnection is not null && hubConnection.State == HubConnectionState.Connected)
        {
            this.message = "Queueing...";
            this.imageUrl = null;
            StateHasChanged();
            this.promptId = await hubConnection.InvokeAsync<Guid>("Prompt", promptInput, negativePrompt, this.controlImage?.Base64Hash);
        }
        else
        {
            this.message = "Clicked While Disconnected";
            StateHasChanged();
        }
    }
    public bool IsConnected =>
        hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            this.message = "Disposed...";
            StateHasChanged();
            await hubConnection.SendAsync("StopListening");
            await hubConnection.DisposeAsync();
        }
        await disposalTokenSource.CancelAsync();
    }

}