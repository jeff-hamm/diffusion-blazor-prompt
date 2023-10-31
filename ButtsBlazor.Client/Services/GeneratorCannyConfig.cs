namespace ButtsBlazor.Client.Services;

public class GeneratorCannyConfig
{
    public int? ControlImgSize { get; set; }
    public bool? Scale { get; set; }
    public bool? ScaleUp { get; set; }
    public bool? Crop { get; set; }
    public string? ScaleDimension { get; set; }
    public int? CannyLow { get; set; }
    public int? CannyHigh { get; set; }
}