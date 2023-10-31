using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using DataUtils;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Client.Services;

public class ButtsApiClient(IHttpClientFactory clientFactory, IOptions<PromptOptions> options) : IButtsApiClient
{

    public async Task<UploadResult> UploadFile(string dataUrlString)
    {
        var dataUrl = new DataUrl(dataUrlString);
        // Get content as string
        using var stream = new MemoryStream(
            Convert.FromBase64String(Encoding.ASCII.GetString(dataUrl.Content)));
        using var client = clientFactory.CreateClient(); 
        return await UploadFile(client,stream , dataUrl.ContentType, 
            Path.ChangeExtension(Path.GetRandomFileName(), "png"));
    }
    public async Task<UploadResult> UploadFile(IBrowserFile file)
    {
        using var client = clientFactory.CreateClient();
        var stream = file.OpenReadStream(options.Value.MaxFileSize);
        return await UploadFile(client, stream, file.ContentType, file.Name);
    }

    private static async Task<UploadResult> UploadFile(HttpClient client, Stream stream, string contentType, string name)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);
        content.Add(
            content: fileContent,
            name: "\"file\"",
            fileName: name);


        var response = await client.PostAsync("api/butts/upload", content);
        return await response.Content.ReadFromJsonAsync<UploadResult>() ??
               throw new InvalidOperationException($"Upload unexpectedly return null");
    }

    public async Task<WebPath[]> GetRecentImages(int numImages)
    {
        using var client = clientFactory.CreateClient();
        var response = await client.GetAsync("/api/butts/recent?count=" + numImages);
        return await response.Content.ReadFromJsonAsync<WebPath[]>() ?? Array.Empty<WebPath>();
    }
}