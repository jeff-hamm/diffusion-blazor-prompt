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

    public async Task<UploadResult> UploadFile(string dataUrlString, string prompt, string inputImage, string code)
    {
        var dataUrl = new DataUrl(dataUrlString);
        // Get content as string
        using var stream = new MemoryStream(
            Convert.FromBase64String(Encoding.ASCII.GetString(dataUrl.Content)));
        using var client = clientFactory.CreateClient(); 
        return await UploadFile(client, stream, dataUrl.ContentType, 
            Path.ChangeExtension(Path.GetRandomFileName(), "png"),
                new[]
                {
                    new KeyValuePair<string, string>("imageType", ImageType.Output.ToString()),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("prompt", prompt),
                    new KeyValuePair<string, string>("inputImage", inputImage)
                }
            );
    }
    public async Task<UploadResult> UploadFile(IBrowserFile file)
    {
        using var client = clientFactory.CreateClient();
        var stream = file.OpenReadStream(options.Value.MaxFileSize);
        return await UploadFile(client, stream, file.ContentType, file.Name);
    }

    private static async Task<UploadResult> UploadFile(HttpClient client, Stream stream,string contentType, string name, IEnumerable<KeyValuePair<string,string>>? additionalFormData=null)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);
        content.Add(
            content: fileContent,
            name: "\"file\"",
            fileName: name);
        //if (additionalFormData != null)
        //    content.Add(new FormUrlEncodedContent(additionalFormData));

        var response = await client.PostAsync("api/butts/upload", content);
        return await response.Content.ReadFromJsonAsync<UploadResult>() ??
               throw new InvalidOperationException($"Upload unexpectedly return null");
    }

    public async Task<WebPath[]> GetRecentImages(int numImages, ImageType? type=null)
    {
        using var client = clientFactory.CreateClient();
        var uri = "/api/butts/recent?count=" + numImages;
        if (type.HasValue)
        {
            uri += "&type=" + type;
        }
        var response = await client.GetAsync(uri);
        return await response.Content.ReadFromJsonAsync<WebPath[]>() ?? Array.Empty<WebPath>();
    }
}