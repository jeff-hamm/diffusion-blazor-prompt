namespace ButtsBlazor.Client.Utils;

public class GenerationOptions
{
    public double PortraitChance { get; set; } = .33;
    public double PrefixChance { get; set; } = .5;
    public double ObjectChance { get; set; } = .5;
    public double SuffixChance { get; set; } = .1;
    public double ElementChance { get; set; } = .5;
    public double PlacesChance { get; set; } = 1.0;
    public double ArtistChance { get; set; } = 1.0;
    public double StyleChance { get; set; } = 1.0;
    public double AdjectiveChance { get; set; } = .5;
    public int NumAdjectives { get; set; } = 1;
    public double ImproverChance { get; set; } = 1.0;
    public double ColorChance { get; set; } = .5;
    public double CharacterChance { get; set; } = .5;
    public const string ConfigSection = nameof(GenerationOptions);
}