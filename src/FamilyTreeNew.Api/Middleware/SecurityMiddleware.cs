using FamilyTreeNew.Api.Extensions;
using FamilyTreeNew.BLL.Helpers;
using System.Security.Cryptography;

namespace FamilyTreeNew.Api.Middleware;

/// <summary>
/// 安全中间件扩展方法
/// </summary>
public static class SecurityMiddlewareExtensions
{
    /// <summary>
    /// 注册安全响应头中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }

    /// <summary>
    /// 注册XSS防护中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    public static IApplicationBuilder UseXssProtection(this IApplicationBuilder app)
    {
        return app.UseMiddleware<XssProtectionMiddleware>();
    }
}

/// <summary>
/// 安全响应头中间件，为所有响应添加安全相关的HTTP头，包括CSP、X-Frame-Options、X-Content-Type-Options等
/// CSP策略采用nonce-based机制：每个请求生成唯一的加密随机nonce值，仅允许携带该nonce的script标签执行，
/// 从而在不使用'unsafe-eval'的情况下支持内联脚本，大幅降低XSS攻击风险
/// 注意：已移除'unsafe-eval'指令，因为它允许执行eval()等动态代码，是XSS攻击的常见利用途径
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 生成加密安全的随机nonce值（32字节），用于CSP策略的script-src指令
        // nonce-based CSP机制：每个请求生成唯一nonce，前端script标签需携带相同nonce才能执行
        // 这比'unsafe-inline'更安全，因为攻击者无法预知nonce值来注入恶意脚本
        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        context.Items["CspNonce"] = nonce;

        // CSP策略中已移除'unsafe-eval'：'unsafe-eval'允许eval()/new Function()等动态代码执行，
        // 是XSS攻击的常见利用途径。移除后可能影响依赖eval的旧代码，需确保前端不使用动态代码执行
        var cspPolicy = "default-src 'self'; " +
                        $"script-src 'self' 'nonce-{nonce}' https://cdn.jsdelivr.net; " +
                        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.font.im https://cdn.jsdelivr.net; " +
                        "font-src 'self' https://fonts.gstatic.com https://fonts.font.im https://cdn.jsdelivr.net; " +
                        "img-src 'self' data: blob:; " +
                        "connect-src 'self'; " +
                        "frame-ancestors 'self'; " +
                        "base-uri 'self'; " +
                        "form-action 'self'";

        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
        context.Response.Headers["Content-Security-Policy"] = cspPolicy;
        context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";

        await _next(context);
    }
}

/// <summary>
/// XSS防护中间件，检测POST/PUT/PATCH请求体和查询参数中的XSS攻击载荷
/// 使用XssDetector进行正则匹配检测
/// </summary>
public class XssProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<XssProtectionMiddleware> _logger;

    public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理请求，检测XSS攻击载荷
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var bodyContent = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (XssDetector.ContainsXssPayload(bodyContent))
            {
                _logger.LogWarning("Potential XSS attack detected in request body from IP: {IpAddress}",
                    context.GetClientIpAddress());

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "请求包含不安全的内容",
                    Code = 400
                });
                return;
            }
        }

        foreach (var query in context.Request.Query)
        {
            if (XssDetector.ContainsXssPayload(query.Value.ToString()))
            {
                _logger.LogWarning("Potential XSS attack detected in query string from IP: {IpAddress}",
                    context.GetClientIpAddress());

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "请求参数包含不安全的内容",
                    Code = 400
                });
                return;
            }
        }

        await _next(context);
    }
}
