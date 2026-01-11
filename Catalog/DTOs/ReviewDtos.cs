using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateReviewRequest
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}
