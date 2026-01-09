using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PetStuff.Web.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadPath = "uploads/products";
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<string>> UploadProductImagesAsync(IFormFileCollection files)
        {
            var uploadedUrls = new List<string>();

            if (files == null || files.Count == 0)
            {
                return uploadedUrls;
            }

            var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, _uploadPath);
            
            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                // Dosya uzantısı kontrolü
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    continue; // Geçersiz uzantı, atla
                }

                // Dosya boyutu kontrolü
                if (file.Length > _maxFileSize)
                {
                    continue; // Çok büyük dosya, atla
                }

                // Benzersiz dosya adı oluştur
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadDirectory, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // URL oluştur (wwwroot'tan sonrası)
                var imageUrl = $"/{_uploadPath}/{fileName}";
                uploadedUrls.Add(imageUrl);
            }

            return uploadedUrls;
        }

        public async Task<bool> DeleteProductImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            try
            {
                // URL'den dosya yolunu çıkar
                // Örnek: /uploads/products/image.jpg -> uploads/products/image.jpg
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
