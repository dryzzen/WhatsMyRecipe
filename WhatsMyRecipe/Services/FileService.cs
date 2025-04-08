using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace WhatsMyRecipe.Services
{
    public interface IFileService
    {
        string UploadImage(IFormFile imageFile, string subFolder);
        void DeleteImage(string imagePath);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string UploadImage(IFormFile imageFile, string subFolder)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Only JPG, PNG or GIF images are allowed");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", subFolder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return $"/images/{subFolder}/{fileName}";
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}