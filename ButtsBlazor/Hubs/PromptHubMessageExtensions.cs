using ButtsBlazor.Api.Model;
using Microsoft.AspNetCore.SignalR;

namespace ButtsBlazor.Hubs;

public static class PromptHubMessageExtensions
{

    public static IClientProxy Listeners(this IHubClients<IClientProxy> @this) =>
        @this.Groups(NotifyHub.ListenersGroup);

    public static Task StatusMessage(this IHubClients<IClientProxy> @this, string message) => 
        @this.Listeners().SendAsync("StatusMessage", message);
    public static Task NewControlImage(this IHubClients<IClientProxy> @this, ImageEntity image) =>
        @this.Listeners().SendAsync("NewControlImage", image);
    public static Task NewCameraImage(this IHubClients<IClientProxy> @this, ImageEntity image) =>
        @this.Listeners().SendAsync("NewCameraImage", image);

    public static Task ProcessComplete(this IHubClients<IClientProxy> @this, PromptEntity completed) => 
        @this.Listeners().SendAsync("ProcessComplete", completed.RowId, 
            completed.OutputImage?.Path
            );

    public static IClientProxy Processors(this IHubClients<IClientProxy> @this) =>
        @this.Groups(NotifyHub.ProcessorsGroup);

}