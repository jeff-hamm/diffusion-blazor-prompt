using System.Text;

namespace ButtsBlazor.Client.Services;

public static class PromptExtensions
{
    public static string ToPromptString(this IEnumerable<PromptChoice>? @this)
    {
        if (@this == null) return "";
        var sb = new StringBuilder();
        foreach (var choice in @this)
        {
            var cStr = choice.ToPromptString();
            if (choice.Preselected)
                cStr = $"__{cStr}__";
            sb.Append(cStr);
        }

        return sb.ToString().Replace("&nbsp;", " ");
    }
}