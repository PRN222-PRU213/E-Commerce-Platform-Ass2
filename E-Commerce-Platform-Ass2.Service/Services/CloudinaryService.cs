using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using E_Commerce_Platform_Ass2.Service.Common.Configurations;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public CloudinaryService(IOptions<CloudinarySettings> options)
        {
            _settings = options.Value;
            if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
                string.IsNullOrWhiteSpace(_settings.ApiKey) ||
                string.IsNullOrWhiteSpace(_settings.ApiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is incomplete.");
            }

            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Folder không được để trống.");

            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("File không hợp lệ. Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp).");

            // Validate file size (max 10MB)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
                throw new ArgumentException("File quá lớn. Kích thước tối đa là 10MB.");

            // Validate MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                throw new ArgumentException("File không hợp lệ. MIME type phải là image.");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                return uploadResult.SecureUrl.ToString();

            throw new Exception($"Upload failed: {uploadResult.Error?.Message ?? "Unknown error"}");
        }
    }
}
