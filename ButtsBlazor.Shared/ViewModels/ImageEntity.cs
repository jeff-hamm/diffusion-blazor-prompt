using System.ComponentModel.DataAnnotations;

namespace ButtsBlazor.Api.Model;

public class ImageEntity
{
    [Key]
    public int RowId { get; set; }
    public required WebPath Path { get; set; }
    public required string Base64Hash { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public ImageType Type { get; set; }
    public virtual ICollection<ImageMetadata> ImageMetadata { get; set; } = null!;
}

public class ImageMetadata
{
    [Key]
    public int RowId { get; set; }
    public string? Prompt { get; set; }
    public string? Code { get; set; }
    public string? InputImage { get; set; }
    public int ImageEntityId { get; set; }
    public virtual ImageEntity ImageEntity { get; set; } = null!;


}