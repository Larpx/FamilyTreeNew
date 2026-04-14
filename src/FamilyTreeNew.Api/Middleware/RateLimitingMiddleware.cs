using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using FamilyTreeNew.Api.Extensions;
using FamilyTreeNew.BLL.Helpers;

namespace FamilyTreeNew.Api.Middleware;

/// <summary>
/// 请求限流中间件扩展方法
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    /// <summary>
    /// 注册请求限流中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}

/// <summary>
/// 请求限流中间件，基于客户端IP和请求路径进行滑动窗口限流
/// 登录接口限制为每分钟5次，其他接口限制为每分钟60次
/// 内置过期条目清理机制：每隔5分钟清理过期记录，防止内存泄漏；
/// 当条目总数超过100000时，按最久未使用顺序淘汰，确保内存占用可控
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
    private const int MaxRequestsPerMinute = 60;
    private const int MaxLoginAttemptsPerMinute = 5;
    private static readonly TimeSpan WindowSize = TimeSpan.FromMinutes(1);
    private static DateTime _lastCleanup = DateTime.UtcNow;
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);
    private const int MaxEntries = 100000;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理请求，检查限流规则
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.GetClientIpAddress();
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var key = $"{clientIp}:{path}";

        var isLoginEndpoint = path.Contains("/api/auth/login", StringComparison.OrdinalIgnoreCase);
        var maxRequests = isLoginEndpoint ? MaxLoginAttemptsPerMinute : MaxRequestsPerMinute;

        var now = DateTime.UtcNow;
        CleanupExpiredEntries(now);
        var rateLimitInfo = _rateLimitStore.AddOrUpdate(
            key,
            _ => new RateLimitInfo { Count = 1, WindowStart = now },
            (_, existing) =>
            {
                if (now - existing.WindowStart > WindowSize)
                {
                    return new RateLimitInfo { Count = 1, WindowStart = now };
                }
                existing.Count++;
                return existing;
            });

        if (rateLimitInfo.Count > maxRequests)
        {
            var retryAfter = (int)(WindowSize - (now - rateLimitInfo.WindowStart)).TotalSeconds;

            _logger.LogWarning(
                "Rate limit exceeded for IP: {ClientIp}, Path: {Path}, Count: {Count}, Max: {Max}",
                clientIp, path, rateLimitInfo.Count, maxRequests);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = retryAfter.ToString();
            context.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo.WindowStart.Add(WindowSize).ToString("O");
            context.Response.ContentType = "application/json; charset=utf-8";

            await context.Response.WriteAsJsonAsync(new
            {
                Success = false,
                Message = isLoginEndpoint
                    ? "登录尝试次数过多，请稍后再试"
                    : "请求过于频繁，请稍后再试",
                Code = 429
            });
            return;
        }

        context.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, maxRequests - rateLimitInfo.Count).ToString();
        context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo.WindowStart.Add(WindowSize).ToString("O");

        await _next(context);
    }

    /// <summary>
    /// 清理过期的限流记录，防止内存泄漏
    /// 使用Interlocked.CompareExchange确保线程安全，避免多个请求同时触发清理
    /// 清理策略：
    /// 1. 每隔CleanupInterval（5分钟）执行一次过期条目清理
    /// 2. 当总条目数超过MaxEntries（100000）时，按WindowStart从旧到新淘汰超出部分
    /// </summary>
    /// <param name="now">当前UTC时间</param>
    private static void CleanupExpiredEntries(DateTime now)
    {
        var lastCleanup = _lastCleanup;
        if (now - lastCleanup < CleanupInterval) return;

        if (Interlocked.CompareExchange(ref _lastCleanup, now, lastCleanup) != lastCleanup) return;

        var keysToRemove = new List<string>();
        foreach (var kvp in _rateLimitStore)
        {
            if (now - kvp.Value.WindowStart > WindowSize)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _rateLimitStore.TryRemove(key, out _);
        }

        if (_rateLimitStore.Count > MaxEntries)
        {
            var oldestKeys = _rateLimitStore
                .OrderBy(kvp => kvp.Value.WindowStart)
                .Take(_rateLimitStore.Count - MaxEntries)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in oldestKeys)
            {
                _rateLimitStore.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// 限流信息记录
    /// </summary>
    private class RateLimitInfo
    {
        /// <summary>
        /// 当前窗口内的请求次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 当前窗口的起始时间
        /// </summary>
        public DateTime WindowStart { get; set; }
    }
}
