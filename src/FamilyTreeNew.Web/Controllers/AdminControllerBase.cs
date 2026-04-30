using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// Web层管理后台控制器基类
/// 提供通用的API客户端创建和认证检查功能，消除子控制器中的重复代码
/// </summary>
public abstract class AdminControllerBase : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    protected AdminControllerBase(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// 创建已配置认证头的API客户端实例
    /// </summary>
    /// <returns>带有JWT认证头的HttpClient实例</returns>
    protected HttpClient GetApiClient()
    {
        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        client.BaseAddress = new Uri(apiBaseUrl);

        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    /// <summary>
    /// 检查当前用户是否已登录，未登录则重定向到登录页面
    /// </summary>
    /// <returns>已登录返回true，否则重定向到登录页并返回false</returns>
    protected bool EnsureLoggedIn()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 检查登录状态，未登录时重定向到登录页
    /// </summary>
    /// <returns>已登录返回null，未登录返回重定向结果</returns>
    protected IActionResult? CheckLoginOrRedirect()
    {
        if (!EnsureLoggedIn())
        {
            return RedirectToAction("Login", "Admin");
        }
        return null;
    }
}
