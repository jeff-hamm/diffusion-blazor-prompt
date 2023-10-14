using ButtsBlazor.Client.Utils;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });// Add services to the container.
builder.Services.AddSingleton(s => new PromptOptions());

await builder.Build().RunAsync();
