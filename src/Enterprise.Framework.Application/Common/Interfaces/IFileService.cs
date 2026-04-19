namespace Enterprise.Framework.Application.Common.Interfaces;

public interface IFileService {

Task<FileResponse> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);

    Task<FileDownloadResponse> DownloadAsync(string path, CancellationToken cancellationToken = default);

    Task DeleteAsync(string path, CancellationToken cancellationToken = default);



 record FileUploadRequest(string FileName, Stream Content, string ContentType, string Folder = "uploads");

public record FileResponse(string Path, string FileName, string Extension, long Size);

public record FileDownloadResponse(Stream Content, string ContentType, string FileName);
}



