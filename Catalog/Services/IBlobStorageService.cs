using Microsoft.AspNetCore.Http;

namespace CatalogService.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
}
