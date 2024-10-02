using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Hubs;
using ButtsBlazor.Server.Components;
using ButtsBlazor.Server.Services;
using ButtsBlazor.Services;
using Configuration.EFCore;
using DotNetEnv;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load("./protobooth.env");
var config = builder.AddSiteConfig();
builder.AddButts();
builder.Services.AddSignalR().AddHubOptions<NotifyHub>(opts =>
{
    opts.MaximumReceiveMessageSize = Int32.MaxValue;
});
builder.AddButtsDb(config.DbPath);
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
app.UseDefaultFiles();
app.UseStaticFiles(
    new StaticFileOptions()
{
    
	OnPrepareResponse = ctx => {
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", 
			"Origin, X-Requested-With, Content-Type, Accept");
	},
});
app.UseRouting();
app.UseCors(builder => builder
	.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader()
);
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
