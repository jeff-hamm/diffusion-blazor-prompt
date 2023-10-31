using ButtsBlazor.Api.Model;
using ButtsBlazor.Shared.Services;
using PubSub;

namespace ButtsBlazor.Client.Services;

public class ButtsNotificationClient(Hub pubsub,ButtsHubManager hubManager) : IButtsNotificationClient
{
    private readonly HashSet<object> ownedSubscribers = new();
    public async Task SubscribeImage(object subscriber, Action<ImageEntity> callback)
    {
//        await hubManager.Initialize();
        pubsub.Subscribe(subscriber, callback);
        ownedSubscribers.Add(subscriber);
    }
    public async Task SubscribeImage(object subscriber, Func<ImageEntity,Task> callback)
    {
  //      await hubManager.Initialize();
        pubsub.Subscribe(subscriber, callback);
        ownedSubscribers.Add(subscriber);
    }

    public void UnsubscribeImage(object subscriber)
    {
        pubsub.Unsubscribe(subscriber);
        ownedSubscribers.Remove(subscriber);
    }

    public void Dispose()
    {
        foreach (var subscriber in ownedSubscribers)
            pubsub.Unsubscribe<ImageEntity>(subscriber);
    }
}