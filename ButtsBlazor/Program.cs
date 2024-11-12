using System.Web;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Client.Services;
using ButtsBlazor.Hubs;
using ButtsBlazor.Server.Components;
using ButtsBlazor.Server.Services;
using ButtsBlazor.Services;
using ButtsBlazor.Shared.Services;
using Configuration.EFCore;
using DotNetEnv;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var envFile = System.Environment.GetEnvironmentVariable("DOTENV_PATH") ?? ".env";
if (File.Exists(envFile))
{
    Env.Load(envFile);
}
var config = builder.AddSiteConfig();
builder.AddMqtt(config.Cluster)
    .AddButts();
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

async Task GeneratePrompts(int count)
{
    var r = new Random();
    await using var fw = new StreamWriter("./prompts.txt");
    foreach (var i in Enumerable.Range(0, 10000))
    {
        using var scope = app.Services.CreateScope();
        var ps = scope.ServiceProvider.GetRequiredService<IPromptGenerationService>();

        var p = ps.GetPromptBuilder().Build();
        foreach (var c in p)
        {
            var f = c.Part switch
            {
                PromptPart.Place => "mystical forest",
                PromptPart.Object => "photo booth",
                PromptPart.Butts => "people in a mystical forest",
                _ => ""
            };
            if (!String.IsNullOrEmpty(f))
                c.Choose(f);
            else
                c.Choose(c.Choices[r.Next(c.Choices.Length)]);
        }

        await fw.WriteLineAsync(
            HttpUtility.HtmlDecode(
                p.ToPromptString()) + ", sony fe 12-24mm f/2.8 gm,  32k uhd, alluring, perfect skin, seductive, amazing quality, wallpaper, analog film grain");
    }
    fw.Flush();
}

await GeneratePrompts(10000);
app.Run();
