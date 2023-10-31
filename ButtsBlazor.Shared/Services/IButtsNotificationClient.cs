using ButtsBlazor.Api.Model;

namespace ButtsBlazor.Shared.Services;

public interface IButtsNotificationClient : IDisposable
{
    Task SubscribeImage(object subscriber, Action<ImageEntity> callback);
    Task SubscribeImage(object subscriber, Func<ImageEntity, Task> callback);
    void UnsubscribeImage(object subscriber);
}