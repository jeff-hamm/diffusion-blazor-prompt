namespace ButtsBlazor.Server.Services;



public record ButtImage(string Path, DateTime Created, int Index, bool IsLatest)
{
    public static readonly ButtImage Empty = new("android-chrome-512x512.png", DateTime.MinValue, 0, false);
}