namespace ButtsBlazor.Api.Model;

public class ImageEntity
{
    public Guid Id { get; set; }
    public required string Path { get; set; }
    public required string Base64Hash { get; set; }
    public ImageType Type { get; set; }

}