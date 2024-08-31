using ButtsBlazor.Client.Utils;

namespace ButtsBlazor.Client.Services;

public class SnickPromptGenerationService(Random random, PromptOptions promptOptions) : IPromptGenerationService
{
    private readonly GenerationOptions options = promptOptions.GenerationOptions;
    public Func<int?, IEnumerable<string>> Choices(PromptPart part)
    {
        return count => random.RemoveNext(new List<string>(PromptTokens.ForPart(part)), count);
    }

    public PromptChoiceBuilder GetPromptBuilder() => new(this, promptOptions, random);
    public IEnumerable<PromptChoice> Choices(PromptChoiceBuilder prompt)
    {
        var buttType = random.NextRequired(PromptTokens.ForPart(PromptPart.Butts));

        if (random.NextChance(options.PrefixChance))
            yield return prompt.Add(PromptPart.Prefix | PromptPart.Character, suffix: $"'s {buttType}");
        else
            yield return prompt.Add(PromptPart.Prefix | PromptPart.Character, suffix: $"'s {buttType}");

        yield return prompt.Add(PromptPart.Place, prefix: " in ", suffix: ", ");

        if (random.NextChance(options.SuffixChance))
            yield return prompt.Add(PromptPart.Suffix, suffix: ", ");


        if (random.NextChance(options.ArtistChance)) {
            var artist = prompt.Add(PromptPart.Artist, prefix: "In the style of ", suffix:", ");
//            artist.Choose(artist.Choices.Length > 0 ? random.NextRequired(artist.Choices) : "", preselected: true);
            yield return artist;
        }
        else
            yield return prompt.Add(PromptPart.Style, prefix: "In the style of ", suffix: ", ");


        if (random.NextChance(options.ImproverChance))
        {
            var imp = prompt.Add(PromptPart.Improver, suffix: ", ");
            imp.Choose(imp.Choices.Length > 0 ? random.NextRequired(imp.Choices) : "", preselected: true);
            yield return imp;
        }

        if (random.NextChance(options.AdjectiveChance))
            for (var i = 0; i < options.NumAdjectives; i++)
                yield return prompt.Add(PromptPart.Adjective);



    }

}