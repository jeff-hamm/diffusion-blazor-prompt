using ButtsBlazor.Client;
using ButtsBlazor.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly;
using Polly.Extensions.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif
var root = builder.RootComponents.FirstOrDefault(c => c.ComponentType == typeof(Routes));
builder.RootComponents.Remove(root);
builder.RootComponents.Add<Routes>("#app");
//builder.Services.AddTransient(s => new HttpClient()
//{
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
//});
builder.Services.AddHttpClient("").ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// Add services to the container.
//builder.Services.AddHttpClient("",
//    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
//builder.Services.AddHttpClient()
//    .ConfigureHttpClientDefaults(cb =>
//        cb.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)));
//            .AddPolicyHandler(GetRetryPolicy()))
    //.AddHttpClient<ButtsApiClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    //    .AddPolicyHandler(GetRetryPolicy());
builder.Services.AddClientButts();
//builder.Services.AddTelerikBlazor();
var app = builder.Build();
await app.UseButts();
await app.RunAsync();

//static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//{
    
//    return HttpPolicyExtensions
//        .HandleTransientHttpError()
//        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
//        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
//}