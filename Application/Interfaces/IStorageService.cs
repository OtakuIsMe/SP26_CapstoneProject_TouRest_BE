namespace TouRest.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
