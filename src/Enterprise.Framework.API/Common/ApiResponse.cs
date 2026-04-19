namespace Enterprise.Framework.API.Common;

public sealed class ApiResponse<T> {
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
    public string TraceId { get; init; } = string.Empty;

    public static ApiResponse<T> Ok(T data, string? message = null, string? traceId = null) {
        return new ApiResponse<T> {
            Success = true,
            Message = message ?? "Islem basarili.",
            Data = data,
            TraceId = traceId ?? string.Empty
        };
    }

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null, string? traceId = null) {
        return new ApiResponse<T> {
            Success = false,
            Message = message,
            Errors = errors ?? Array.Empty<string>(),
            TraceId = traceId ?? string.Empty
        };
    }
}
