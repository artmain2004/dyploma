namespace CatalogService.Settings;

public class BlobStorageOptions
{
    public string ConnectionString { get; set; } = "";
    public string ContainerName { get; set; } = "product-images";
}
