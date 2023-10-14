using System.ComponentModel.DataAnnotations;

namespace ButtsBlazor.Services;

public class PromptArgs
{
    [Key]
    public required Guid Id { get; init; }
    public required string Prompt { get; init; }
    public string? ControlFilePath { get; init; }
    public string? ControlFile { get; set; }
    public string? Negative { get; init; }
    public int? NumSteps { get; init; }
    public int? CannyLow { get; init; }
    public int? CannyHigh { get; init; }
    public int? ControlSize { get; init; }
    public double? ControlScale { get; init; }
}