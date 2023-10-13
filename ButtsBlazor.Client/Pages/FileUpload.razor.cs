using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

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

        async Task OnChange(InputFileChangeEventArgs e)
        {
            imageSources.Clear();
            ErrorMessage = string.Empty;

            if (e.FileCount > maxAllowedFiles)
            {
                ErrorMessage = $"Only {maxAllowedFiles} files can be uploaded";
                return;
            }

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                await using var stream = file.OpenReadStream(maxAllowedSize: 1024L * 1024L * 100L);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                imageSources.Add($"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}");
            }

            HoverClass = string.Empty;
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
