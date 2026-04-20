using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using TouRest.Application.Interfaces;

namespace TouRest.Infrastructure.Services
{
    public class StorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        public StorageService()
        {
            var cloudName = Environment.GetEnvironmentVariable("CLOUDIANRY_CLOUD_NAME")!;
            var apiKey    = Environment.GetEnvironmentVariable("CLOUDIANRY_API_KEY")!;
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDIANRY_API_SECRET")!;

            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File           = new FileDescription(file.FileName, stream),
                UseFilename    = false,
                UniqueFilename = true,
                Overwrite      = false,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }

        public async Task<List<string>> UploadManyAsync(List<IFormFile> files)
        {
            var tasks = files.Select(f => UploadAsync(f));
            var urls  = await Task.WhenAll(tasks);
            return urls.ToList();
        }
    }
}
