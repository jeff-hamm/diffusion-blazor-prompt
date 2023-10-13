//using ButtsBlazor.Invokable;
//using Coravel.Queuing.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace ButtsBlazor.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RenderQueue(IQueue queue, ButtRenderTracker tracker) : ControllerBase
//    {
//        public IQueue Queue { get; } = queue;

//        [HttpGet]
//        public ButtQueueViewModel Get() => new ()
//        {
//            // Possible race condition, metrics might not match current. It's fine
//            Metrics = Queue.GetMetrics(),
//            Current = tracker.Current
//        };

//        [HttpPost]
//        public Guid Post([FromBody] ButtPromptArgs? args) =>
//            Queue.QueueInvocableWithPayload<ButtPromptProcess, ButtPromptArgs?>(args);
//    }  
//}
