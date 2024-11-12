using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.ViewModels;
using System.Collections.Generic;

public static class ButtsClientExtensions
{
    
    internal static readonly JsonSerializerOptions s_defaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    public static async ValueTask<T?> ReadFromJsonAsync<T>(this HttpContent @this, JsonSerializerOptions? options=null,  CancellationToken cancellationToken=default)
    {
        using (Stream contentStream = await GetContentStreamAsync(@this, cancellationToken).ConfigureAwait(false))
        {
            return await JsonSerializer.DeserializeAsync<T>(contentStream, options ?? s_defaultSerializerOptions, cancellationToken).ConfigureAwait(false);
        }
    }
    internal static async ValueTask<Stream> GetContentStreamAsync(HttpContent content, CancellationToken cancellationToken)
    {
        Stream contentStream = await ReadHttpContentStreamAsync(content, cancellationToken).ConfigureAwait(false);

        // Wrap content stream into a transcoding stream that buffers the data transcoded from the sourceEncoding to utf-8.
        if (GetEncoding(content) is Encoding sourceEncoding && sourceEncoding != Encoding.UTF8)
        {
            contentStream = GetTranscodingStream(contentStream, sourceEncoding);
        }

        return contentStream;
    }
    internal static Encoding? GetEncoding(HttpContent content)
    {
        Encoding? encoding = null;

        if (content.Headers.ContentType?.CharSet is string charset)
        {
            try
            {
                // Remove at most a single set of quotes.
                if (charset.Length > 2 && charset[0] == '\"' && charset[charset.Length - 1] == '\"')
                {
                    encoding = Encoding.GetEncoding(charset.Substring(1, charset.Length - 2));
                }
                else
                {
                    encoding = Encoding.GetEncoding(charset);
                }
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException("CharSetInvalid", e);
            }

            Debug.Assert(encoding != null);
        }

        return encoding;
    }
    private static Task<Stream> ReadHttpContentStreamAsync(HttpContent content, CancellationToken cancellationToken)
    {
        return content.ReadAsStreamAsync();
    }

    private static Stream GetTranscodingStream(Stream contentStream, Encoding sourceEncoding)
    {
        return contentStream;
    }








}
public interface IButtsApiClient
{
    Task<UploadResult> UploadFile(string dataUrlString,string prompt, string inputImage, string code, ImageType? imageType=null);
    Task<WebPath[]> GetRecentImages(int numImages, ImageType? type = null);

    public Task<UploadResult> UploadFile(Stream stream, string contentType, string name,
        IEnumerable<KeyValuePair<string, string>>? additionalFormData = null);

}