using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using DataUtils;

namespace ButtsBlazor.Client.Services;

public class ButtsApiClient(Func<HttpClient> clientFactory) : IButtsApiClient
{

    public async Task<UploadResult> UploadFile(string dataUrlString, string prompt, string inputImage, string code,
        ImageType? imageType = null)
    {
        var dataUrl = new DataUrl(dataUrlString);
        // Get content as string
        using var stream = new MemoryStream(
            Convert.FromBase64String(Encoding.ASCII.GetString(dataUrl.Content)));
        return await UploadFile(stream, dataUrl.ContentType,
            Path.ChangeExtension(Path.GetRandomFileName(), "png"),
                new[]
            {
                new KeyValuePair<string, string>("imageType", (imageType ?? ImageType.Output).ToString()),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("prompt", prompt),
                new KeyValuePair<string, string>("inputImage", inputImage)
            }
            );
    }

    public async Task<UploadResult> UploadFile(Stream stream, string contentType, string name, IEnumerable<KeyValuePair<string, string>>? additionalFormData = null)
    {
        using var client = clientFactory();
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);
        content.Add(
            content: fileContent,
            name: "\"file\"",
            fileName: name);
        if (additionalFormData != null)
            content.Add(new FormUrlEncodedContent(additionalFormData));

        var response = await client.PostAsync("api/butts/upload", content);
        if(await response.Content.ReadFromJsonAsync<UploadResult>() is not {} result)
               throw new InvalidOperationException($"Upload unexpectedly return null");
        result.BaseUrl = client.BaseAddress;
        return result;
    }

    public async Task<WebPath[]> GetRecentImages(int numImages, ImageType? type=null)
    {
        using var client = clientFactory();
        var uri = "/api/butts/recent?count=" + numImages;
        if (type.HasValue)
        {
            uri += "&type=" + type;
        }
        var response = await client.GetAsync(uri);
        return await response.Content.ReadFromJsonAsync<WebPath[]>() ?? Array.Empty<WebPath>();
    }
}