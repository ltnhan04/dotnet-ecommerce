using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace api.Utils
{
    public class CloudinaryUtils
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryUtils(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<List<string>> UploadImage(List<IFormFile> files)
        {
            var imageUrls = new List<string>();
            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "products"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    imageUrls.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    throw new AppException($"Upload failed: {uploadResult.Error?.Message}");
                }
            }
            return imageUrls;
        }
        public async Task DeleteImage(List<string> imageUrls)
        {
            foreach (var imageUrl in imageUrls)
            {
                var publicId = GetPublicIdFromUrl(imageUrl);
                var deletionParams = new DeletionParams($"products/{publicId}");
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                if (deletionResult.Result != "ok")
                {
                    Console.WriteLine($"Failed to delete image: {imageUrl} - {deletionResult.Error?.Message}");
                }
            }
        }
        private static string GetPublicIdFromUrl(string url)
        {
            var fileName = Path.GetFileNameWithoutExtension(new Uri(url).AbsolutePath);
            return fileName;
        }
    }
}