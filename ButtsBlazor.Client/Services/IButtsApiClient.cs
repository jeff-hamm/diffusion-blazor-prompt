using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

namespace ButtsBlazor.Client.Services;

public interface IButtsApiClient
{
    Task<UploadResult> UploadFile(string dataUrlString,string prompt, string inputImage, string code);
    Task<UploadResult> UploadFile(IBrowserFile file);
    Task<WebPath[]> GetRecentImages(int numImages, ImageType? type = null);
}