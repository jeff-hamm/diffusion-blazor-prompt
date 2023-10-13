//using System.Collections.Concurrent;
//using System.Threading.Channels;
//using ButtsBlazor.Hubs;
//using Coravel.Queuing.Broadcast;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Options;
//using PubSub;
//using Hub = PubSub.Hub;

//namespace ButtsBlazor.Invokable;

//public record StatusMessageEvent(Guid Id, string Message);
//public record OutputFileEvent(Guid Id, string OutputFile);

//public class ButtRenderTracker 
//{
//    private readonly Hub pubSubHub = new Hub();
//    private readonly ILogger<ButtRenderTracker> logger;
//    private readonly IOptions<ButtOptions> options;
//    private readonly ConcurrentQueue<RenderingButt> history = new();

//    public ButtRenderTracker(ILogger<ButtRenderTracker> logger, IOptions<ButtOptions> options)
//    {
//        this.logger = logger;
//        this.options = options;
//    }

//    private RenderingButt? _current;
//    private readonly object syncLock = new();
//    public RenderingButt? Current => _current;

//    public void OnMessage(object subscriber,Func<StatusMessageEvent,Task> messageHandler)
//    {
//        pubSubHub.Subscribe(subscriber, messageHandler);
//    }
//    public void OnNewImage(object subscriber, Func<OutputFileEvent, Task> handler)
//    {
//        pubSubHub.Subscribe(subscriber, handler);
//    }
//    public void StatusMessage(Guid id, string message)
//    {
//        lock (syncLock)
//        {
//            if (_current?.Id == id)
//            {
//                _current.Value.Messages.Enqueue(message);
//            }
//            else
//                logger.LogWarning($"Unable to record message for process {id}, no longer current {message}");
//        }
//        pubSubHub.Publish(new StatusMessageEvent(id, message));
//    }
//    public Task OutputFile(Guid id, string outputFile)
//    {
//        lock (syncLock)
//        {
//            if (id == _current?.Id)
//            {
//                _current = _current.Value with
//                {
//                    OutputFile = outputFile
//                };
//            }
//        }
//        pubSubHub.Publish(new OutputFileEvent(id,outputFile));
//        return Task.CompletedTask;
//    }


//    public Task HandleAsync(QueueTaskStarted broadcasted)
//    {
//        lock (syncLock)
//        {
//            CompleteCurrent();
//            _current = new RenderingButt(broadcasted.Guid);
//        }

//        return Task.CompletedTask;
//    }

//    public Guid ProcessStarted(ButtPromptProcess buttPromptProcess)
//    {
//        lock (syncLock)
//        {
//            var cts = new CancellationTokenSource();
//            buttPromptProcess.Token = cts.Token;
//            var details = new RenderingButtDetails(DateTime.UtcNow, buttPromptProcess, buttPromptProcess.Payload,cts);
//            if (_current?.IsPending == true)
//            {
//                _current = _current.Value with
//                {
//                    Detail = details
//                };
//            }
//            else 
//            {
//                logger.LogError($"Unexpected processing, expected a pending task, assigning a random Guid");
//                _current = new RenderingButt(Guid.NewGuid(), details);
//            }
//            return _current.Value.Id;
//        }
//    }


//    private void CompleteCurrent()
//    {
//        if (_current == null) return;
//        lock (syncLock)
//        {
//            if (_current != null)
//            {
//                if (history.Count > options.Value.TrackerHistoryLength)
//                    history.TryDequeue(out _);
//                _current.Value.Detail?.TokenSource?.Dispose();
//                history.Enqueue(_current.Value);
//            }

//            _current = null;
//        }
//    }

//    public Task HandleAsync(QueueTaskCompleted broadcasted)
//    {

//        lock (syncLock)
//        {
//            if (broadcasted.Guid == _current?.Id)
//            {
//                _current = _current.Value with
//                {
//                    Completed = DateTime.UtcNow
//                };
//            }

//            CompleteCurrent();
//        }

//        return Task.CompletedTask;
//    }

//    public Task HandleAsync(DequeuedTaskFailed broadcasted)
//    {
//        lock (syncLock)
//        {
//            if (broadcasted.Guid == _current?.Id)
//                _current = _current.Value with
//                {
//                    Completed = DateTime.UtcNow,
//                    Failed = true
//                };
//            else
//                CompleteCurrent();
//        }

//        return Task.CompletedTask;
//    }

//    public void HandleException(Exception exception)
//    {
//        lock (syncLock)
//        {
//            if (_current?.Failed == true)
//            {
//                _current = _current.Value with
//                {
//                    Exception = exception
//                };
//            }

//            CompleteCurrent();
//        }
//    }

//    public void Unsubscribe<T>(object subscriber)
//    {
//        pubSubHub.Unsubscribe<T>(subscriber);
//    }

//}