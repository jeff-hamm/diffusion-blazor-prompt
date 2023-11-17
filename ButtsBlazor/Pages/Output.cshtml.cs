using System.Diagnostics.CodeAnalysis;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Components;
using ButtsBlazor.Client.Pages;
using ButtsBlazor.Client.Services;
using ButtsBlazor.Server.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ButtsBlazor.Server.Pages;

public class OutputModel : PageModel
{
    private readonly FileService fileService;

    public OutputModel(FileService fileService)
    {
        this.fileService = fileService;
    }
    public int CalculatedMaxWidth { get; set; }

    public async Task OnGet()
    {
        CalculatedMaxWidth = MaxWidth ?? (Columns * ItemWidth) + ((Columns - 1) * GridGap);
        await Reload();
    }



    public bool Animate { get; set; } = false;

    public bool Zoom { get; set; }

    public bool Masonry { get; set; }



    public int GridGap { get; set; } = 1;

    public int ItemWidth { get; set; } = 100;

    public int ItemRounding { get; set; } = 6;

    public string ActionIcon { get; set; } = "\ud83c\udf51";

    public int? MaxWidth { get; set; }

    public string ExtraClasses { get; set; } = "";


    public int SelectedIndex { get; set; }
    public int Columns { get; set; } = 6;

    public int Rows { get; set; } = 4;

    public int NumImages { get; set; } = 30;
    public List<ButtImage>? images;
    private ImageGrid? grid;

    private async Task Reload()
    {
        IsLoading = true;
        grid?.Clear();
        images = new();
        await foreach(var image in fileService.GetLatest(NumImages, ImageType.Output))
            images.Add(image);
        await foreach (var image in fileService.GetLatest(NumImages, ImageType.Camera))
            images.Add(image);
        images.Sort((one,two) => two.Created.CompareTo(one.Created));
        IsLoading = false;
    }

    public bool IsLoading { get; set; } = true;

}
