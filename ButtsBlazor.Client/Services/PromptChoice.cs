namespace ButtsBlazor.Client.Services;

public record class PromptChoice(PromptChoiceBuilder Prompt, PromptPart Part,
    string[] Choices, string? Prefix = null, string? Suffix = null)
{
    public string? Choice { get; set; }
    public bool Preselected { get; set; }
    public void Choose(string choice, bool preselected=false)
    {
        Preselected = preselected;
        Choice = choice;
    }

    public string ToPromptString()
    {
        if (String.IsNullOrEmpty(Choice))
            return "";
        return $"{Prefix}{Choice}{Suffix}";
    }

    public override string ToString()
    {
        var content = Choice;
        if (string.IsNullOrEmpty(content))
            content = $"[{String.Join(',', Choices)}]";

        return $"{Prefix}{content}{Suffix}";
    }
}