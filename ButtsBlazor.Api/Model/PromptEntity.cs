using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ButtsBlazor.Services;

namespace ButtsBlazor.Api.Model;

public class PromptEntity
{
    [Key]
    public int RowId { get; set; }
    public int ArgsId { get; set; }
    public virtual required PromptArgs Args { get; set; }
    public required DateTime Enqueued { get; set; }
    public int? ControlImageId { get; set; }
    public virtual ImageEntity? ControlImage { get; set; }
    public int? CannyImageId { get; set; }
    public virtual ImageEntity? CannyImage { get; set; }
    public int? OutputImageId { get; set; }
    public virtual ImageEntity? OutputImage { get; set; }

    public DateTime? ProcessingStart { get; set; }
    public DateTime? ProcessingCompleted { get; set; }
}