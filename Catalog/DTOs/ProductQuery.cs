namespace CatalogService.DTOs;

public class ProductQuery
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? Featured { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
