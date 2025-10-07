using Microsoft.AspNetCore.Http;

namespace ChatApp.Backend.Services;

public interface IFileUploadService
{
    Task<string> SaveFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    bool IsImageFile(IFormFile file);
    bool IsVideoFile(IFormFile file);
    long MaxImageSize { get; }
    long MaxVideoSize { get; }
}

