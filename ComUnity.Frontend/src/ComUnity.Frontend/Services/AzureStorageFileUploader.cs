using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace ComUnity.Frontend.Services;

public class AzureStorageFileUploader : IAzureStorageFileUploader
{
    public async Task<Guid> UploadFile(string sasToken, IBrowserFile file, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var blobServiceClient = new BlobServiceClient(new Uri(sasToken));
        var containerClient = blobServiceClient.GetBlobContainerClient("pictures");
        var blobClient = containerClient.GetBlobClient(id.ToString());

        await blobClient.UploadAsync(file.OpenReadStream(cancellationToken: cancellationToken), new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType, } }, cancellationToken);

        return id;
    }
}

public interface IAzureStorageFileUploader
{
    Task<Guid> UploadFile(string sasToken, IBrowserFile file, CancellationToken cancellationToken);
}