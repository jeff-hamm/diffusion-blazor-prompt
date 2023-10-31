using System.Text;

namespace ButtsBlazor.Client.Services;

public static class PromptExtensions
{
    public static string ToPromptString(this IEnumerable<PromptChoice>? @this)
    {
        if (@this == null) return "";
        var sb = new StringBuilder();
        foreach (var choice in @this)
            sb.Append(choice.ToPromptString());
        return sb.ToString();
    }
}