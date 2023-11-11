using System.Runtime.CompilerServices;
using ButtsBlazor.Api.Model;

namespace ButtsBlazor.Server.Services;



public record ButtImage(WebPath Path, DateTime Created, int Index, WebPath? ThumbnailPath)
{
    public static readonly ButtImage Empty = new(new WebPath("android-chrome-512x512.png"), DateTime.MinValue, 0, null);
//    public static ButtImage From(ImageEntity entity) => new ButtImage(entity.Path, entity.CreationDate, entity.RowId);

    public bool IsLatest { get; set; }
}