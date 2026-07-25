using MyShop.Business.Interfaces;

namespace MyShop.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
            };

        private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File size cannot exceed 2 MB.");

            var extension = Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Only JPG, JPEG, PNG, and WEBP files are allowed.");

            var rootPath = _webHostEnvironment.WebRootPath; // => wwwroot

            var uploadPath = Path.Combine(rootPath, "uploads", "products");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"uploads/products/{fileName}";
        }

        public Task DeleteAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.CompletedTask;

            var rootPath = _webHostEnvironment.WebRootPath;

            var fullPath = Path.Combine(
                rootPath,
                filePath.TrimStart('\\', '/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}
