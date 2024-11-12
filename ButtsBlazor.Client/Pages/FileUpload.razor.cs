using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ButtsBlazor.Client.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using ButtsBlazor.Client.Services;
namespace ButtsBlazor.Client.Pages
{
    partial class FileUpload : ComponentBase
    {
        ElementReference fileDropContainer;
        InputFile? inputFile;

        IJSObjectReference? _filePasteModule;
        IJSObjectReference? _filePasteFunctionReference;

        private string? HoverClass;
        private List<string> imageSources = new();
        private const int maxAllowedFiles = 2;
        private string? ErrorMessage;
        private bool isLoading;
        private UploadResult? uploadResult;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _filePasteModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "../Pages/FileUpload.razor.js");

                _filePasteFunctionReference = await _filePasteModule.InvokeAsync<IJSObjectReference>("initializeFilePaste", fileDropContainer, inputFile?.Element);
            }
        }

        void OnDragEnter(DragEventArgs e) => HoverClass = "hover";

        void OnDragLeave(DragEventArgs e) => HoverClass = string.Empty;

        private async Task OnChange(InputFileChangeEventArgs e)
        {
            ErrorMessage = string.Empty;

            var file = e.File;
            try
            {
                isLoading = true;
                StateHasChanged();
                using var client = ClientFactory.CreateClient();
                uploadResult = await Client.UploadFile(file,Options);
                if (uploadResult?.Uploaded == true && uploadResult.Path != null)
                    imageSources.Add(uploadResult.Path);

            }
            catch (Exception ex)
            {
                Logger.LogInformation(
                    "{FileName} not uploaded: {Message}",
                    file.Name, ex.Message);
            }
            finally
            {
                isLoading = false;
                HoverClass = string.Empty;
                StateHasChanged();
            }
        }


        public async ValueTask DisposeAsync()
        {
            if (_filePasteFunctionReference != null)
            {
                await _filePasteFunctionReference.InvokeVoidAsync("dispose");
                await _filePasteFunctionReference.DisposeAsync();
            }
            if (_filePasteModule != null)
            {
                await _filePasteModule.DisposeAsync();
            }
        }

    }
}
