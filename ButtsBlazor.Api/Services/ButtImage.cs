using ButtsBlazor.Api.Model;

namespace ButtsBlazor.Server.Services;



public record ButtImage(WebPath Path, DateTime Created, int Index, bool IsLatest)
{
    public static readonly ButtImage Empty = new(new WebPath("android-chrome-512x512.png"), DateTime.MinValue, 0, false);
}