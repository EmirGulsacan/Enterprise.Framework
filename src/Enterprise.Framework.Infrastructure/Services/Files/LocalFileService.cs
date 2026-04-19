namespace Enterprise.Framework.Infrastructure.Services.Files;

using Enterprise.Framework.Application.Common.Interfaces;

using Microsoft.AspNetCore.Hosting;



public class LocalFileService : IFileService {

private readonly string _basePath;

    public LocalFileService(IWebHostEnvironment env) {
    
_basePath = Path.Combine(env.ContentRootPath, "Storage");

        if (!Directory.Exists(_basePath)) {
        
Directory.CreateDirectory(_basePath);

        }

    

 async Task<FileResponse> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default) {
    
var extension = Path.GetExtension(request.FileName);

        var uniqueFileName = $"
Guid.NewGuid():N}

extension}
";

        var folderPath = Path.Combine(_basePath, request.Folder);

        if (!Directory.Exists(folderPath)) {
        
Directory.CreateDirectory(folderPath);

        }

        var fullPath = Path.Combine(folderPath, uniqueFileName);

        var relativePath = Path.Combine(request.Folder, uniqueFileName);

        using (var fileStream = new FileStream(fullPath, FileMode.Create)) {
        
await request.Content.CopyToAsync(fileStream, cancellationToken);

        }

        return new FileResponse(
            Path: relativePath,
            FileName: request.FileName,
            Extension: extension,
            Size: request.Content.Length
        );

    

 Task<FileDownloadResponse> DownloadAsync(string path, CancellationToken cancellationToken = default) {
    
var fullPath = Path.Combine(_basePath, path);

        if (!File.Exists(fullPath)) {
        
throw new FileNotFoundException("Dosya bulunamadı", path);

        }

        var fileName = Path.GetFileName(fullPath);

        var content = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

        var contentType = "application/octet-stream";

        return Task.FromResult(new FileDownloadResponse(content, contentType, fileName));

    

 Task DeleteAsync(string path, CancellationToken cancellationToken = default) {
    
var fullPath = Path.Combine(_basePath, path);

        if (File.Exists(fullPath)) {
        
File.Delete(fullPath);

        }

        return Task.CompletedTask;

    }

}
}



