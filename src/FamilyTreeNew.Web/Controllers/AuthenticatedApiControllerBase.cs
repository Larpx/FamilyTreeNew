using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 已认证 API 控制器基类。
/// 这个基类封装了 Web 前端调用后端 API 时常用的认证、错误处理和会话清理逻辑。
/// </summary>
public abstract class AuthenticatedApiControllerBase : Controller
{
    /// <summary>
    /// `HttpClientFactory`。
    /// 用于创建调用后端 API 的 `HttpClient` 实例。
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// 应用配置。
    /// 用于读取 API 地址等运行时配置。
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 构造函数。
    /// </summary>
    protected AuthenticatedApiControllerBase(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// 创建已配置好基础地址和 Bearer 令牌的 API 客户端。
    /// </summary>
    protected HttpClient GetApiClient()
    {
        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        client.BaseAddress = new Uri(apiBaseUrl);

        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    /// <summary>
    /// 将错误消息写入 `TempData`。
    /// </summary>
    protected void SetErrorMessage(string message)
    {
        TempData["Error"] = message;
    }

    /// <summary>
    /// 将成功消息写入 `TempData`。
    /// </summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["Success"] = message;
    }

    /// <summary>
    /// 向模型状态添加一条通用错误。
    /// </summary>
    protected void AddErrorMessage(string message)
    {
        ModelState.AddModelError(string.Empty, message);
    }

    /// <summary>
    /// 从 API 响应中提取错误信息，并显示为页面错误提示。
    /// </summary>
    protected async Task SetResponseErrorMessageAsync(HttpResponseMessage response, string fallbackMessage)
    {
        var apiResponse = await ReadApiResponseAsync(response);
        SetErrorMessage(apiResponse?.Message ?? fallbackMessage);
    }

    /// <summary>
    /// 从 API 响应中提取多个错误信息，并逐条写入模型状态。
    /// </summary>
    protected async Task AddResponseErrorsAsync(HttpResponseMessage response, string fallbackMessage)
    {
        var apiResponse = await ReadApiResponseAsync(response);
        if (apiResponse?.Errors.Count > 0)
        {
            foreach (var error in apiResponse.Errors)
            {
                AddErrorMessage(error);
            }

            return;
        }

        AddErrorMessage(apiResponse?.Message ?? fallbackMessage);
    }

    /// <summary>
    /// 尝试把 API 响应解析成统一的响应模型。
    /// 如果内容为空或格式错误，则返回 `null`。
    /// </summary>
    private static async Task<ApiResponse?> ReadApiResponseAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<ApiResponse>(content);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 检查当前用户是否已通过 Cookie 和 JWT 完成认证。
    /// 如果未认证或令牌失效，则跳回登录页。
    /// </summary>
    protected async Task<IActionResult?> EnsureAuthenticatedAsync()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            await SignOutAndClearSessionAsync();
            return RedirectToAction("Login", "Admin");
        }

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
        {
            await SignOutAndClearSessionAsync();
            return RedirectToAction("Login", "Admin");
        }

        return null;
    }

    /// <summary>
    /// 处理后端返回的 401 未授权响应。
    /// 如果发生未授权，就清理会话并重定向到登录页。
    /// </summary>
    protected async Task<IActionResult?> HandleUnauthorizedResponseAsync(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return null;
        }

        await SignOutAndClearSessionAsync();
        return RedirectToAction("Login", "Admin");
    }

    /// <summary>
    /// 判断 JWT 是否已经过期。
    /// </summary>
    protected static bool IsTokenExpired(string token)
    {
        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 清空会话并注销 Cookie 认证状态。
    /// </summary>
    protected async Task SignOutAndClearSessionAsync()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
