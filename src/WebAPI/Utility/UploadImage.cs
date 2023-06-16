using Azure.Storage.Blobs;

public class UploadImage
{
    private readonly BlobContainerClient blobServiceClient;

    public UploadImage(BlobContainerClient _blobContainerClient)
    {
        blobServiceClient = _blobContainerClient;
    }

    public async Task<string> UploadToCloud(IFormFile file)
    {
        string filename= Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        BlobClient blobClient = blobServiceClient.GetBlobClient(filename);
        await blobClient.UploadAsync(file.OpenReadStream(), true);
        return blobClient.Uri.ToString();
    }
}
