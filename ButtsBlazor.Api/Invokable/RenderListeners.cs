//using Coravel.Events.Interfaces;
//using Coravel.Queuing.Broadcast;

//namespace ButtsBlazor.Invokable;

//public class RenderListeners
//{
//    public class Started(ButtRenderTracker renderTracker) : IListener<QueueTaskStarted>
//    {
//        public Task HandleAsync(QueueTaskStarted broadcasted) => renderTracker.HandleAsync(broadcasted);
//    }

//    public class Completed(ButtRenderTracker renderTracker) : IListener<QueueTaskCompleted>
//    {
//        public Task HandleAsync(QueueTaskCompleted broadcasted) => renderTracker.HandleAsync(broadcasted);
//    }
//    public class Failed(ButtRenderTracker renderTracker) : IListener<DequeuedTaskFailed>
//    {
//        public Task HandleAsync(DequeuedTaskFailed broadcasted) => renderTracker.HandleAsync(broadcasted);
//    }

//}