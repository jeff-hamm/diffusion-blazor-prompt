namespace ButtsBlazor.Api.Model;

public interface IPathConverter
{
    FilePath ToFilePath(WebPath webPath);
    WebPath ToWebPath(FilePath webPath);
}

// DI break. Bite me.
public static class PathConverterAccessor
{
    private static IPathConverter? converter;
    public static IPathConverter PathConverter => converter ?? throw new ArgumentNullException(nameof(converter));
    public static void SetPathConverter(IPathConverter value) => converter = value;
}
