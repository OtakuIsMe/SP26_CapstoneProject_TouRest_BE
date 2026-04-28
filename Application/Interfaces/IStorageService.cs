using Microsoft.AspNetCore.Http;

namespace TouRest.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadAsync(IFormFile file);
        Task<List<string>> UploadManyAsync(List<IFormFile> files);
    }
}
