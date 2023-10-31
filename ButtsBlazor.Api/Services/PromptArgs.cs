using System.ComponentModel.DataAnnotations;
using ButtsBlazor.Api.Model;

namespace ButtsBlazor.Services;

public class PromptArgs
{
    [Key]
    public int RowId { get; init; }
    public required string Prompt { get; init; }
    public WebPath? ControlFilePath { get; init; }
    public string? ControlFile { get; set; }
    public string? Negative { get; init; }
    public int? NumSteps { get; init; }
    public int? CannyLow { get; init; }
    public int? CannyHigh { get; init; }
    public int? ControlSize { get; init; }
    public double? ControlScale { get; init; }
}