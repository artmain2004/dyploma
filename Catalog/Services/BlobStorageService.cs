using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CatalogService.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CatalogService.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobStorageOptions _options;

    public BlobStorageService(IOptions<BlobStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("Image file is empty.");
        }

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.ContainerName))
        {
            throw new InvalidOperationException("Azure Blob Storage container name is not configured.");
        }

        var serviceClient = new BlobServiceClient(_options.ConnectionString);
        var container = serviceClient.GetBlobContainerClient(_options.ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var extension = Path.GetExtension(file.FileName);
        var blobName = $"{Guid.NewGuid():N}{extension}";
        var blobClient = container.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        var headers = new BlobHttpHeaders { ContentType = file.ContentType };
        await blobClient.UploadAsync(stream, headers, cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }
}
