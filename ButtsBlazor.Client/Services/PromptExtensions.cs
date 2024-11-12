using ButtsBlazor.Client.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using System.Text;
using ButtsBlazor.Client.Utils;

namespace ButtsBlazor.Client.Services;

public static class PromptExtensions
{
    public static async Task<UploadResult> UploadFile(this IButtsApiClient @this, IBrowserFile file, PromptOptions options)
    {
        var stream = file.OpenReadStream(options.MaxFileSize);
        return await @this.UploadFile(stream, file.ContentType, file.Name);
    }

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