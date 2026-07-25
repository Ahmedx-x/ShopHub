using Microsoft.AspNetCore.Http;

namespace MyShop.Business.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string filePath);
    }
}