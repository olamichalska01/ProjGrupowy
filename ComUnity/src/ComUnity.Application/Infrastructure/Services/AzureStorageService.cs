using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ComUnity.Application.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ComUnity.Application.Infrastructure.Services;

internal class AzureStorageService : IAzureStorageService
{
    private readonly AzureStorageSettings _storageSettings;

    public AzureStorageService(IOptions<AzureStorageSettings> storageSettings)
    {
        _storageSettings = storageSettings.Value;
    }

    public async Task<string> GenerateNewWriteToken()
    {
        var blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _storageSettings.ContainerName,
            StartsOn = DateTime.UtcNow.AddMinutes(-1),
            ExpiresOn = DateTime.UtcNow.AddMinutes(15),
        };
        blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Write);
        var sasToken = blobSasBuilder.ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(
            _storageSettings.AccountName,
            _storageSettings.AccountKey)).ToString();

        await Task.CompletedTask;
        return $"{_storageSettings.BlobServerUrl}?{sasToken}";
    }

    public string GetReadFileToken(Guid pictureId)
    {
        var blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _storageSettings.ContainerName,
            BlobName = pictureId.ToString(),
            StartsOn = DateTime.UtcNow.AddMinutes(-1),
            ExpiresOn = DateTime.UtcNow.AddMinutes(15),
        };
        blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
        var sasToken = blobSasBuilder.ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(
            _storageSettings.AccountName,
            _storageSettings.AccountKey)).ToString();

        return $"{_storageSettings.BlobServerUrl}/{_storageSettings.ContainerName}/{pictureId}?{sasToken}";
    }

    public bool PictureExists(Guid pictureId)
    {
        var client = new BlobServiceClient(_storageSettings.StorageConnectionString);
        var blob = client.GetBlobContainerClient(_storageSettings.ContainerName).GetBlobClient(pictureId.ToString());

        return blob.Exists().Value;
    }
}

public interface IAzureStorageService
{
    Task<string> GenerateNewWriteToken();

    bool PictureExists(Guid pictureId);

    string GetReadFileToken(Guid pictureId);
}
