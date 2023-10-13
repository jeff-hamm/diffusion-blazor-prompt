using Microsoft.AspNetCore.Components;

namespace ButtsBlazor.Client.Pages;

partial class Gallery : ComponentBase
{
    [Parameter]
    public int ActiveIndex { get; set; }
    [Parameter]
    public IList<string> Images { get; set; } = Array.Empty<string>();
}