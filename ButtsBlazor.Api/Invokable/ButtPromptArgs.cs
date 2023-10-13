namespace ButtsBlazor.Invokable;

public class ButtPromptArgs
{
    public static ButtPromptArgs Default { get; } = new()
    {
        Prompt =
            "a rear view of a butt, holding a futuristic cyborg tool, on the moon and with asteroids in the sky",
        NegativePrompt = "low quality, bad quality, sketches",
        ControlImageName = "butts.png",
        OutputFileName = "output",
        ControlImageSize = 768,
        CannyLowThreshold = 80,
        CannyHighThreshold = 100,
        ConditioningScale = .5
    };

    public string? Prompt { get; set; }
    public string? NegativePrompt { get; set; }
    public string? ControlImageName { get; set; }
    public int? ControlImageSize { get; set; }
    public int? CannyLowThreshold { get; set; }
    public int? CannyHighThreshold { get; set; }
    public double? ConditioningScale { get; set; }
    public string? OutputFileName { get; set; }
}

