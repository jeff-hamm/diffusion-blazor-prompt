using ButtsBlazor.Client.Utils;

namespace ButtsBlazor.Client.Services;

public class PromptChoiceBuilder(IPromptGenerationService service, PromptOptions options, Random random)
{
    // public PromptSection? PortraitType { get; set; }
    // public PromptSection? Prefix { get; set; }
    // public PromptSection? Object { get; set; }
    // public PromptSection Artists { get; set; }
    // public PromptSection Styles { get; set; }
    // public PromptSection? Suffix { get; set; }
    // public IReadOnlyList<PromptChoice> Choices => _choices ?? BuildChoices();
    public int NumItemsPerPrompt { get; set; } = options.NumItemsPerChoice;

    public PromptChoice[] Build()
    {
        _choices = new List<PromptChoice>();
        var s = service.Choices(this).ToArray();
 
        return s;
    }
    internal PromptChoice Add(PromptPart part, string? prefix=null, string? suffix=null)
    {
        if(_choices == null)
            _choices = new List<PromptChoice>();
        var c = new PromptChoice(this, part, service.Choices(part)(NumItemsPerPrompt).ToArray(), 
            prefix?.Replace(" ", "&nbsp;"), suffix?.Replace(" ", "&nbsp;"));
        _choices.Add(c);
        return c;
    }

    private List<PromptChoice>? _choices;

    public override string ToString() => _choices.ToPromptString();

}