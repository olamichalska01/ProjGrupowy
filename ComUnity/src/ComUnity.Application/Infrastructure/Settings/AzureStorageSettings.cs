namespace ComUnity.Application.Infrastructure.Settings;

public class AzureStorageSettings
{
    public string BlobServerUrl { get; set; }
    public string StorageConnectionString { get; set; }
    public string ContainerName { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
}
