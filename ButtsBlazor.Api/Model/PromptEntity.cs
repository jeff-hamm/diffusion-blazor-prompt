using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ButtsBlazor.Services;

namespace ButtsBlazor.Api.Model;

public class PromptEntity
{
    [Key, ForeignKey(nameof(PromptArgs))]
    public required Guid Id { get; set; }
    public virtual required PromptArgs Args { get; set; }
    public required DateTimeOffset Enqueued { get; set; }
    public Guid? ControlImageId { get; set; }
    public virtual ImageEntity? ControlImage { get; set; }
    public Guid? CannyImageId { get; set; }
    public virtual ImageEntity? CannyImage { get; set; }
    public Guid? OutputImageId { get; set; }
    public ImageEntity? OutputImage { get; set; }

    public DateTimeOffset? ProcessingStart { get; set; }
    public DateTimeOffset? ProcessingCompleted { get; set; }
}