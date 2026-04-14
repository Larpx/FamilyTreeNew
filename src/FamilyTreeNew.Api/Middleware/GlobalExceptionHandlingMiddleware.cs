using System.Text.Json;
using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.Api.Middleware;

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}

/// <summary>
/// 全局异常处理中间件
/// 安全设计：在生产环境中不向客户端暴露异常详细信息（如堆栈跟踪、内部错误消息），
/// 防止攻击者利用异常细节获取系统内部信息（如文件路径、数据库结构、框架版本等），
/// 仅在Development环境下返回原始异常消息以便调试
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request: {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 处理异常并根据环境决定返回信息的详细程度
    /// 安全目的：生产环境中隐藏异常详情，仅返回通用错误消息，
    /// 避免泄露服务器内部信息（文件路径、SQL语句、框架版本等），
    /// 这些信息可能被攻击者用于进一步构造攻击
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="exception">捕获的异常</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        switch (exception)
        {
            case ArgumentException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(
                    isDevelopment ? exception.Message : "请求参数错误", 400), _jsonOptions);
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(
                    isDevelopment ? exception.Message : "未授权访问", 401), _jsonOptions);
                break;
            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(
                    isDevelopment ? exception.Message : "操作无效", 400), _jsonOptions);
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(
                    isDevelopment ? exception.Message : "服务器内部错误，请稍后重试", 500), _jsonOptions);
                break;
        }
    }
}
