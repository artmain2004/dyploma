namespace CatalogService.DTOs;

public class ProductListResponse
{
    public List<ProductListItemDto> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
