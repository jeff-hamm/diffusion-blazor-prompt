using ButtsBlazor.Api.Model;
using ButtsBlazor.Shared.Services;

namespace ButtsBlazor.Api.Services;

public class ServerNotificationClient : IButtsNotificationClient
{
    public void Dispose()
    {
    }

    public Task SubscribeImage(object subscriber, Action<ImageEntity> callback)
    {
        return Task.CompletedTask;
    }

    public Task SubscribeImage(object subscriber, Func<ImageEntity, Task> callback)
    {
        return Task.CompletedTask;
    }

    public void UnsubscribeImage(object subscriber)
    {
    }
}