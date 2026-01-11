using CatalogService.Data;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Middleware;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class ReviewService : IReviewService
{
    private readonly CatalogDbContext _db;

    public ReviewService(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReviewDto>> GetReviewsAsync(Guid productId)
    {
        return await _db.Reviews.AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                UserName = r.UserName,
                CreatedAtUtc = r.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<ReviewDto> AddReviewAsync(Guid productId, CreateReviewRequest request, string userId, string? userName)
    {
        var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            throw new ApiException(404, "Product not found");
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            UserId = userId,
            UserName = userName,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            UserName = review.UserName,
            CreatedAtUtc = review.CreatedAtUtc
        };
    }
}
