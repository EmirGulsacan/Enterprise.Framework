namespace Enterprise.Framework.API.Middleware;

using FluentValidation;
using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Domain.Common.Exceptions;
using System.Text.Json;

public sealed class GlobalExceptionMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        }
        catch (ValidationException validationException) {
            var validationErrors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Validasyon hatası.", null, validationErrors);
        }
        catch (NotFoundException notFoundException) {
            _logger.LogWarning(notFoundException, "Kayit bulunamadi: {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, notFoundException.Message);
        }
        catch (DomainException domainException) {
            _logger.LogWarning(domainException, "Domain kurali ihlali: {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, domainException.Message);
        }
        catch (BusinessRuleException businessRuleException) {
            _logger.LogWarning(businessRuleException, "Is kurali ihlali: {ErrorCode} - {TraceId}", businessRuleException.ErrorCode, context.TraceIdentifier);
            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, businessRuleException.Message, new[] { businessRuleException.ErrorCode });
        }

        catch (InvalidOperationException invalidOperationException) {
            _logger.LogWarning(invalidOperationException, "Is kurali hatasi olustu: {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, invalidOperationException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException) {
            _logger.LogWarning(unauthorizedAccessException, "Yetki hatasi olustu: {TraceId}", context.TraceIdentifier);
            var statusCode = context.User?.Identity?.IsAuthenticated == true ? StatusCodes.Status403Forbidden : StatusCodes.Status401Unauthorized;
            await WriteErrorAsync(context, statusCode, unauthorizedAccessException.Message);
        }
        catch (BadHttpRequestException badHttpRequestException) {
            _logger.LogWarning(badHttpRequestException, "Hatali istek: {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Geçersiz istek formatı veya hatalı veri.");
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Beklenmeyen hata olustu: {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "Sunucu hatasi olustu.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message, IReadOnlyList<string>? errors = null, IDictionary<string, string[]>? validationErrors = null) {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var response = ApiResponse<object>.Fail(message, errors, validationErrors, context.TraceIdentifier);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
