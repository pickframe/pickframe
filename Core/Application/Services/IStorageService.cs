namespace Application.Services;

public interface IStorageService
{
    Task<bool> UploadFileAsync(string path, MemoryStream memoryStream);
    Task DownloadFileAsync(string path, string localDirectory);
    Task<string> GetPublicURL(string path);
}
