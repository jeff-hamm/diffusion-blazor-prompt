using ButtsBlazor.Invokable;
using Coravel.Queuing;

namespace ButtsBlazor.Controllers;

public class ButtQueueViewModel
{
    public required QueueMetrics Metrics { get; set; }
    public RenderingButt? Current { get; set; }
}