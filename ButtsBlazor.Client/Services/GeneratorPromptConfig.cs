namespace ButtsBlazor.Client.Services;

public class GeneratorPromptConfig
{
    public int? NumOutputs { get; set; }
    public int? ImgSize { get; set; }
    public int? NumSteps { get; set; }
    public double? ControlScale { get; set; }
    public GeneratorCannyConfig CannyConfig { get; set; } = new();
}