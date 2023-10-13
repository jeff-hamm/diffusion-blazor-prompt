using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ButtsBlazor.Client.Pages
{
    partial class ImageCard : ComponentBase
    {
        [Parameter]
        public string? Url { get; set; }

        public string? Title { get; set; }
    }
}
