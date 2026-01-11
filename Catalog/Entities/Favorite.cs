namespace CatalogService.Entities;

public class Favorite
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public Product Product { get; set; } = null!;
}
