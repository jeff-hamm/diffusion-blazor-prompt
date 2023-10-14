using Microsoft.AspNetCore.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ButtsBlazor.Client.Components
{
    partial class ImageCard : ComponentBase
    {
        [Parameter] public int? MaxDimension { get; set; }

        public string Classes { get; set; } = "";
        [Parameter]
        public string? Src { get; set; }
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? InputAttributes { get; set; }
        public string? Title { get; set; }

        protected override void OnParametersSet()
        {
            if (MaxDimension.HasValue)
            {
                Classes = "image-card-" + MaxDimension;
            }

            base.OnParametersSet();
        }
    }
}
