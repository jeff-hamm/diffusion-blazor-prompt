using ButtsBlazor.Client.Utils;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Client.Services;

public class ButtsOptionsProvider(IHttpClientFactory clientFactory) : IOptions<PromptOptions>
{

    public async Task<PromptOptions> GetOptions()
    {
        using var client = clientFactory.CreateClient();
        var response = await client.GetAsync("api/butts/options");
        return promptOptions = await response.Content.ReadFromJsonAsync<PromptOptions>() ??
               throw new InvalidOperationException($"Upload unexpectedly return null");
    }


    private PromptOptions? promptOptions;
    public PromptOptions Value => promptOptions ??= GetOptions().GetAwaiter().GetResult();
}