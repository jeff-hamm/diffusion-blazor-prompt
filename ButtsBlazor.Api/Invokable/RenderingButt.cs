using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace ButtsBlazor.Invokable;

public readonly record struct RenderingButt(Guid Id, RenderingButtDetails? Detail=null, string? OutputFile=null, DateTime? Completed = null, bool Failed=false, Exception? Exception=null)
{
    public readonly ConcurrentQueue<string> Messages = new();
    public bool IsPending => Detail == null && Completed == null && !Failed;
    public bool IsRunning => Detail != null && Completed == null && !Failed;
    public bool IsComplete => !IsPending && !IsRunning;
    public bool IsSuccess => Completed.HasValue && !Failed;
    public bool IsFailed => Failed || Exception != null;
}


public record RenderingButtDetails(DateTime Started, [property: JsonIgnore] ButtPromptProcess Process, ButtPromptArgs? Args, [property: JsonIgnore]CancellationTokenSource TokenSource);
