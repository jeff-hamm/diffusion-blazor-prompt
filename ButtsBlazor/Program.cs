using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Hubs;
using ButtsBlazor.Server.Components;
using ButtsBlazor.Server.Services;
using ButtsBlazor.Services;
using Configuration.EFCore;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.Configure<ButtOptions>(builder.Configuration.GetSection(nameof(ButtOptions)));
//builder.Services.AddButtPrompts();
builder.Services.AddButts(builder.Configuration);
builder.Services.AddSignalR().AddHubOptions<NotifyHub>(opts =>
{
    opts.MaximumReceiveMessageSize = Int32.MaxValue;
});
builder.Services.AddButtsDb();

builder.Configuration.AddEFCoreConfiguration<ButtsDbContext>(options =>
{
    options.UseSqlite(ButtsDbContext.DefaultConnectionString);
}, reloadOnChange: true, onLoadException: ex => ex.Ignore = true);
builder.Services.AddControllers();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation()
    ;
//builder.Services.AddHttpClient("").ConfigureHttpClient((sp,c) =>
//{
//    var client = new HttpClient();
//    var uri = sp.GetService<IWebAssemblyHostEnvironment>()?.BaseAddress;
//    if (uri != null)
//        client.BaseAddress = new Uri(uri);
//    else
//    {
//        var request = sp.GetService<HttpContext>();
//        if (request?.Request?.Host.Value is not { } host)
//            host = "locahost:5023";
//        client.BaseAddress = new Uri($"http://{host}");
//    }

//});
builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents()
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
//app.UseButtPrompts();

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.MapHub<NotifyHub>("/notify");
app.MapRazorPages();
app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Input).Assembly);
await app.Services.MigrateDatabase();
app.Run();
