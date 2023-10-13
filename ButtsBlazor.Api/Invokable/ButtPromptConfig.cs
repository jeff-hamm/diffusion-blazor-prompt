//using ButtsBlazor.Hubs;
//using Coravel;
//using Coravel.Queuing.Broadcast;
//using Coravel.Queuing.Interfaces;

//namespace ButtsBlazor.Invokable;

//public static class ButtPromptConfig
//{
//    public static IServiceCollection AddButtPrompts(this IServiceCollection services)
//    {
//        services.AddQueue()
//            .AddEvents()
//            .AddSingleton<ButtRenderTracker>()
//            .AddSingleton<PromptMessageListener>()
//            .AddTransient<ButtPromptProcess>()
//            .AddTransient<RenderListeners.Started>()
//            .AddTransient<RenderListeners.Completed>()
//            .AddTransient<RenderListeners.Failed>();
//        return services;
//    }
//    public static IApplicationBuilder UseButtPrompts(this IApplicationBuilder app)
//    {
//        var provider = app.ApplicationServices;
//        var reg = provider.ConfigureEvents();
//        reg.Register<QueueTaskStarted>().Subscribe<RenderListeners.Started>();
//        reg.Register<QueueTaskCompleted>().Subscribe<RenderListeners.Completed>();
//        reg.Register<DequeuedTaskFailed>().Subscribe<RenderListeners.Failed>();
//        var state = provider.GetRequiredService<ButtRenderTracker>();
//        provider.ConfigureQueue().OnError(state.HandleException).LogQueuedTaskProgress(provider.GetRequiredService<ILogger<IQueue>>());
//        return app;
//    }
//}