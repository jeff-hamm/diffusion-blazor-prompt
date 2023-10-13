using ButtsBlazor.Invokable;
using Microsoft.AspNetCore.SignalR;

namespace ButtsBlazor.Hubs;

public class PromptMessageListener
{
    public PromptMessageListener(ButtRenderTracker tracker, IHubContext<PromptHub> context)
    {
        Tracker = tracker;
        Context = context;
    }

    public ButtRenderTracker Tracker { get; }
    public IHubContext<PromptHub> Context { get; }
    private bool GlobalConnection { get; set; }
    public void Listen(string connectionId, CancellationToken token)
    {
        if (!GlobalConnection)
        {
            GlobalConnection = true;
            Tracker.OnMessage(this, message => OnStatusMessage(Context.Clients.All, message));
            Tracker.OnNewImage(this, path => OnNewImage(Context.Clients.All, path));
        }
        var client = Context.Clients.Client(connectionId);
        Tracker.OnMessage(client, message => OnStatusMessage(Context.Clients.Client(connectionId), message));
        Tracker.OnNewImage(client, path => OnNewImage(Context.Clients.Client(connectionId), path));
        token.Register(() => Disconnect(connectionId));
    }

    private async Task OnNewImage(IClientProxy proxy, OutputFileEvent outputFile)
    {
        await proxy.SendAsync("ReceiveImage", outputFile.OutputFile);
    }

    public void Disconnect(string connectionId)
    {
        var client = Context.Clients.Client(connectionId);
        Tracker.Unsubscribe<StatusMessageEvent>(client);
    }

    public async Task OnStatusMessage(IClientProxy proxy, StatusMessageEvent message)
    {
        await proxy.SendAsync("ReceiveMessage", message.Id, message.Message);
    }

}