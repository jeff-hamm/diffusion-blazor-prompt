using ButtsBlazor.Client.Utils;
using ButtsBlazor.Invokable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Services
{
    public static class PromptConfig
    {
        public static IServiceCollection AddPrompts(this IServiceCollection @this, IConfigurationManager configurationManager)
        {
            @this.AddSingleton<FileService>();
            @this.Configure<PromptOptions>(configurationManager.GetSection(nameof(PromptOptions)));
            @this.AddSingleton(sp => sp.GetService<IOptions<PromptOptions>>()?.Value ?? throw new InvalidOperationException($"Could not find PromptOptions"));
            @this.AddSingleton<PromptQueue>();
            return @this;
        }
    }
}
