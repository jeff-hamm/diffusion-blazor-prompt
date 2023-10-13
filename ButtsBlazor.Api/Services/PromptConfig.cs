using ButtsBlazor.Client.Utils;
using ButtsBlazor.Invokable;

namespace ButtsBlazor.Services
{
    public static class PromptConfig
    {
        public static WebApplicationBuilder AddPrompts(this WebApplicationBuilder @this)
        {
            @this.Services.AddSingleton<FileService>();
            @this.Services.Configure<PromptOptions>(@this.Configuration.GetSection(nameof(PromptOptions)));
            @this.Services.AddSingleton<PromptQueue>();
            return @this;
        }
    }
}
