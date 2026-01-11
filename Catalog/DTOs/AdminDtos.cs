using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace CatalogService.DTOs;

public class AdminCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class AdminCategoryCreateRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
}

public class AdminCategoryUpdateRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
}

public class AdminProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public Guid? CategoryId { get; set; }
}

public class AdminProductCreateRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public bool IsFeatured { get; set; }

    public Guid? CategoryId { get; set; }
}

public class AdminProductUpdateRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public bool IsFeatured { get; set; }

    public Guid? CategoryId { get; set; }
}
