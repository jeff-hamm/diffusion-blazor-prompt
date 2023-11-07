using System.Security.Cryptography;
using System;
using System.Diagnostics.CodeAnalysis;
using static ButtsBlazor.Client.Services.PromptTokens;
using System.Numerics;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Client.Utils;

namespace ButtsBlazor.Client.Services;

public class PromptGenerationService(Random random, PromptOptions promptOptions)
{
    private readonly GenerationOptions options = promptOptions.GenerationOptions;
    internal Func<int?,IEnumerable<string>> Choices(PromptPart part)
    {
        return count => random.RemoveNext(new List<string>(ForPart(part)), count);
    }

    public PromptChoiceBuilder GetPromptBuilder() => new (this, promptOptions,random);
    internal IEnumerable<PromptChoice> Choices(PromptChoiceBuilder prompt)
    {
        if (random.NextChance(options.PortraitChance))
        {
            var portrait = prompt.Add(PromptPart.PortraitType, suffix: " of ");
            portrait.Choose(portrait.Choices.Length > 0 ? random.NextRequired(portrait.Choices) : "", preselected: true);
            yield return portrait;
        }

        var character = random.NextChance(options.CharacterChance);
        var prefix = random.NextChance(options.PrefixChance);

        if (prefix)
            yield return prompt.Add(PromptPart.Prefix, suffix: character ? " " : " Butt");
        if (character)
            yield return prompt.Add(PromptPart.Character, suffix: "'s butt");
        else if (!prefix)
            yield return prompt.Add(PromptPart.Butts);

        var place = random.NextChance(options.PlacesChance);
        if (!prefix)
        {
            var suffix = place ? "" : ",";
            if (random.NextChance(options.ObjectChance))
                yield return prompt.Add(PromptPart.Object, prefix: " with ", suffix);
            else if (random.NextChance(options.ElementChance))
                yield return prompt.Add(PromptPart.Element, prefix: " of ", suffix);
        }

        if (place)
            yield return prompt.Add(PromptPart.Place, prefix: " in ", suffix: ", ");

        if (random.NextChance(options.SuffixChance))
            yield return prompt.Add(PromptPart.Suffix, suffix: ", ");


        var hasStyle = random.NextChance(options.StyleChance);
  //      var hasArtist = random.NextChance(options.ArtistChance);
        if (hasStyle)
        {
            yield return prompt.Add(PromptPart.Style, prefix: "In the style of ", suffix: ", ");
//            if (hasArtist)
//                yield return prompt.Add(PromptPart.Artist, prefix: " in ", suffix: " style");
        }
//        else if (hasArtist)
//            yield return prompt.Add(PromptPart.Artist, prefix: ", ", suffix: " style");

        if (random.NextChance(options.AdjectiveChance))
            for (var i = 0; i < options.NumAdjectives; i++)
                yield return prompt.Add(PromptPart.Adjective, suffix: ", ");

        if (random.NextChance(options.ColorChance))
            yield return prompt.Add(PromptPart.Color, suffix: ", ");
        if (random.NextChance(options.ImproverChance))
            yield return prompt.Add(PromptPart.Improver);


    }

}