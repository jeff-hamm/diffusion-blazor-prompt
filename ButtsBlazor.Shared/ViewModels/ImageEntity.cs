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
}