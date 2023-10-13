using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace ButtsBlazor.Client.Pages;

partial class Prompt : ComponentBase
{
    private HubConnection? hubConnection;
    private string? imageUrl;
    private string? message;
    private string? promptInput;
    private string? negativePrompt;

    readonly CancellationTokenSource disposalTokenSource = new();
    private UploadResult? uploadResult;
    private bool isLoading;
    private List<string> images = new() { "/input/butts.png" };

    protected override async Task OnInitializedAsync()
    {
        // await webSocket.ConnectAsync(new Uri("ws://localhost:5555/status"), disposalTokenSource.Token);
        // receiveLoop = ReceiveLoop();
        hubConnection = new HubConnectionBuilder()
            .ConfigureLogging(lb => lb.SetMinimumLevel(LogLevel.Debug))
            .WithUrl(Navigation.ToAbsoluteUri("/prompt"))
            .WithAutomaticReconnect()
            .Build();
        hubConnection.Reconnecting += async s =>
        {
            this.message = "Reconnecting";
            await InvokeAsync(StateHasChanged);
        };
        hubConnection.Reconnected += async s =>
        {
            this.message = "Reconnected";
            await InvokeAsync(StateHasChanged);
        };

        hubConnection.On<string>("StatusMessage", (status) =>
        {
            status = status.Trim();
            if (!String.IsNullOrEmpty(status))
            {
                this.message = status;
                StateHasChanged();
            }
        });
        hubConnection.On<Guid, string>("ProcessComplete", (id, url) =>
        {
            this.processedId = id;
            Console.WriteLine("ProcessComplete");
            this.message = $"Completed: {id}";
            this.imageUrl = url + "?cache=" + DateTime.Now.Ticks;
            images.Add(this.imageUrl);
            StateHasChanged();
        });

        message = "Connecting...";
        StateHasChanged();
        await hubConnection.StartAsync();
        await hubConnection.SendAsync("StartListening");
        message = "Connected.";
        StateHasChanged();
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

    PromptOptions options = new();
    private byte[]? uploadHash;
    private Guid? promptId;
    private Guid? processedId;

    public async Task Send()
    {

        if (hubConnection is not null && hubConnection.State == HubConnectionState.Connected)
        {
            this.message = "Queueing...";
            this.imageUrl = null;
            StateHasChanged();
            this.promptId = await hubConnection.InvokeAsync<Guid>("Prompt", promptInput, negativePrompt, this.uploadResult?.Hash);
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

    private async Task OnInputFileChange(InputFileChangeEventArgs e)
    {

        using var content = new MultipartFormDataContent();
        var file = e.File;
        try
        {
            isLoading = true;
            StateHasChanged();
            using var sha256 = SHA256.Create();
            await using (var hashingStream = new CryptoStream(file.OpenReadStream(options.MaxFileSize), sha256, CryptoStreamMode.Read))
            {
                var fileContent = new StreamContent(hashingStream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(file.ContentType);

                content.Add(
                    content: fileContent,
                    name: "\"file\"",
                    fileName: file.Name);


                var response = await Http.PostAsync("api/Files", content);
                uploadResult = await response.Content.ReadFromJsonAsync<UploadResult>();
                if (uploadResult?.Uploaded == true && uploadResult.Path != null)
                    images.Add(uploadResult.Path);
            }
            uploadHash = sha256.Hash;

        }
        catch (Exception ex)
        {
            Logger.LogInformation(
                "{FileName} not uploaded: {Message}",
                file.Name, ex.Message);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

}