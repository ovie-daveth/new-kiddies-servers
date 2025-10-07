using Microsoft.AspNetCore.Http;

namespace ChatApp.Backend.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;

    // File size limits (in bytes)
    public long MaxImageSize => 10 * 1024 * 1024; // 10MB
    public long MaxVideoSize => 100 * 1024 * 1024; // 100MB

    private readonly string[] _allowedImageExtensions = { 
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif", ".svg", ".bmp", ".tiff", ".tif", ".heic", ".heif" 
    };
    
    private readonly string[] _allowedVideoExtensions = { 
        // Video formats
        ".mp4", ".mov", ".avi", ".webm", ".mkv", ".flv", ".wmv", ".m4v", ".3gp", ".mpeg", ".mpg",
        // Audio formats (for video posts with audio)
        ".mp3", ".wav", ".ogg", ".m4a", ".aac", ".flac", ".wma"
    };

    public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL path
            return $"/uploads/{folder}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file");
            throw new Exception("Failed to save file", ex);
        }
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            // Remove leading slash if present
            filePath = filePath.TrimStart('/');
            
            var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, filePath);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return Task.FromResult(false);
        }
    }

    public bool IsImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedImageExtensions.Contains(extension);
    }

    public bool IsVideoFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedVideoExtensions.Contains(extension);
    }
}

