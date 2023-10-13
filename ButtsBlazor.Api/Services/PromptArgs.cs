namespace ButtsBlazor.Services;

public record PromptArgs(Guid Id, string Prompt, string? Negative = null, string? ControlFilePath = null,
    int? CannyLow = null, int? CannyHigh = null, int? ControlSize = null,
    double? ControlScale = null)
{
    public string? ControlFile { get; set; }
    public DateTimeOffset? Enqueued { get; set; }
    public DateTimeOffset? ProcessingStart { get; set; }
    public DateTimeOffset? ProcessingCompleted { get; set; }
    public string? ImagePath { get; set; }
}