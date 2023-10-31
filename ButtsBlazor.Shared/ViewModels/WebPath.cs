using System.Text.Json;
using System.Text.Json.Serialization;

namespace ButtsBlazor.Api.Model;

[JsonConverter(typeof(WebPathJsonConverter))]
public struct WebPath : IEquatable<WebPath>
{
    public WebPath(string webPath)
    {
        if (String.IsNullOrEmpty(webPath))
            throw new ArgumentNullException(nameof(webPath));
        this.Path = webPath;
    }
    public string Path { get; set; }
    public FilePath FilePath => PathConverterAccessor.PathConverter.ToFilePath(this);
    public override string ToString() => Path;

    public static implicit operator string(WebPath p) => p.Path;
    public static explicit operator FilePath(WebPath p) => p.FilePath;

    public bool Equals(WebPath other)
    {
        return Path == other.Path;
    }

    public override bool Equals(object? obj)
    {
        return obj is WebPath other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    public static bool operator ==(WebPath left, WebPath right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WebPath left, WebPath right)
    {
        return !left.Equals(right);
    }
}

public class WebPathJsonConverter : JsonConverter<WebPath>
{
    public override WebPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new WebPath(reader.GetString() ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, WebPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Path);
    }
}