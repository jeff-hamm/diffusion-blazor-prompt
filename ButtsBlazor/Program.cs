using ButtsBlazor.Components;
using ButtsBlazor.Hubs;
using ButtsBlazor.Invokable;
using ButtsBlazor.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ButtOptions>(builder.Configuration.GetSection(nameof(ButtOptions)));
builder.Services.AddButtPrompts();
builder.AddPrompts();
builder.Services.AddSignalR().AddHubOptions<PromptHub>(opts =>
{
    opts.MaximumReceiveMessageSize = Int32.MaxValue;
});
//builder.Services.AddResponseCompression(opts =>
//{
//    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
//        new[] { "application/octet-stream" });
//});
builder.Services.AddControllers();
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient();
    var uri = sp.GetService<IWebAssemblyHostEnvironment>()?.BaseAddress;
    if (uri != null)
        client.BaseAddress = new Uri(uri);
    else
        client.BaseAddress = new Uri("http://localhost:5023");
    return client;
});
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();
//app.UseResponseCompression();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseButtPrompts();

//app.UseHttpsRedirection();

app.UseStaticFiles();
//app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.MapHub<PromptHub>("/prompt");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.Run();
