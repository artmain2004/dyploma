using CatalogService.DTOs;

namespace CatalogService.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetReviewsAsync(Guid productId);
    Task<ReviewDto> AddReviewAsync(Guid productId, CreateReviewRequest request, string userId, string? userName);
}
