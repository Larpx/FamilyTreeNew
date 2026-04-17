using Microsoft.AspNetCore.Http;

namespace FamilyTreeNew.Api.Extensions;

/// <summary>
/// HttpContext扩展辅助类，提供客户端IP地址获取功能
/// </summary>
public static class HttpContextHelper
{
    /// <summary>
    /// 获取客户端IP地址，优先从X-Forwarded-For请求头获取（适用于反向代理场景）
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>客户端IP地址，无法获取时返回"Unknown"</returns>
    public static string GetClientIpAddress(this HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var forwardedIp = forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(forwardedIp))
            {
                ipAddress = forwardedIp;
            }
        }

        return ipAddress ?? "Unknown";
    }
}
