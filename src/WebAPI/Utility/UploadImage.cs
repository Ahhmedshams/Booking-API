using Azure;
using Azure.Storage.Blobs;

public class UploadImage
{
    private readonly BlobContainerClient blobServiceClient;

    public UploadImage(BlobContainerClient _blobContainerClient)
    {
        blobServiceClient = _blobContainerClient;
    }

    public async Task<List<ImageEntity>> UploadToCloud(IEnumerable<IFormFile> files, string entityType)
    {
        var images = new List<ImageEntity>();

        foreach (var file in files)
        {
            string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            BlobClient blobClient = blobServiceClient.GetBlobClient(filename);

            try
            {
                await blobClient.UploadAsync(file.OpenReadStream(), true);
                var uri = blobClient.Uri.ToString();

                ImageEntity image;
                switch (entityType)
                {
                    case "ServiceImage":
                        image = new ServiceImage { Uri = uri };
                        break;
                    case "ResourceTypeImage":
                        image = new ResourceTypeImage { Uri = uri };
                        break;
                    case "ResourceImage":
                        image = new ResourceImage { Uri = uri };
                        break;
                    case "UserImage":
                        image = new UserImage { Uri = uri };
                        break;
                    default:
                        // Throw an exception or handle the error in some other way اتصررااف
                        throw new ArgumentException("Invalid entity type", nameof(entityType));
                }
                images.Add(image);
            }
            catch (RequestFailedException ex)
            {
                // Handle upload error
                // Log the error, return a custom error message, or retry the upload اعمل اي حاجه متسكتلووش
                Console.WriteLine($"Error uploading image: {ex.Message}");
            }
        }

        return images;
    }



    /* public async Task<List<string>> UploadToCloud(IEnumerable<IFormFile> files)
     {
         var imageUrls = new List<string>();

         foreach (var file in files)
         {
             string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
             BlobClient blobClient = blobServiceClient.GetBlobClient(filename);
             try
             {
                 await blobClient.UploadAsync(file.OpenReadStream(), true);
                 imageUrls.Add(blobClient.Uri.ToString());
             }
             catch(RequestFailedException ex)
             {
                 Console.WriteLine($"Error uploading image: {ex.Message}");
                 // Handle upload error
             }
         }

         return imageUrls;

         *//*string filename= Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
         BlobClient blobClient = blobServiceClient.GetBlobClient(filename);
         try
         {
             await blobClient.UploadAsync(file.OpenReadStream(), true);
             return blobClient.Uri.ToString();
         }
         catch
         {
             return null;
         }*//*
     }*/
}
