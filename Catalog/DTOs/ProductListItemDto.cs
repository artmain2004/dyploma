namespace CatalogService.DTOs;

public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public double AverageRating { get; set; }
    public int ReviewsCount { get; set; }
}
