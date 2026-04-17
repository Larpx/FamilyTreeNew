using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 负责展示和保存后台系统设置页面。
/// </summary>
public class SettingsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SettingsController> _logger;

    /// <summary>
    /// 初始化系统设置控制器。
    /// </summary>
    /// <param name="httpClientFactory">创建 API 请求客户端的工厂。</param>
    /// <param name="configuration">读取 API 地址等配置的配置对象。</param>
    /// <param name="logger">记录错误和诊断信息的日志器。</param>
    public SettingsController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SettingsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private HttpClient GetApiClient()
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
    /// 打开系统设置首页，并从 API 读取当前设置数据填充页面。
    /// </summary>
    /// <returns>系统设置页面。</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/system/settings");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<SystemSettingsDto>>(content);
                
                if (result?.Data != null)
                {
                    var updateDto = new UpdateSystemSettingsDto
                    {
                        SiteName = result.Data.SiteName,
                        SiteDescription = result.Data.SiteDescription,
                        LogoUrl = result.Data.LogoUrl,
                        FaviconUrl = result.Data.FaviconUrl,
                        ThemeColor = result.Data.ThemeColor,
                        ShowStatistics = result.Data.ShowStatistics,
                        AllowGuestBrowse = result.Data.AllowGuestBrowse,
                        RequireVerification = result.Data.RequireVerification,
                        MaxLoginAttempts = result.Data.MaxLoginAttempts,
                        LockoutDuration = result.Data.LockoutDuration,
                        SessionTimeout = result.Data.SessionTimeout,
                        EnableOperationLog = result.Data.EnableOperationLog,
                        LogRetentionDays = result.Data.LogRetentionDays,
                        ContactEmail = result.Data.ContactEmail,
                        ContactPhone = result.Data.ContactPhone,
                        ContactAddress = result.Data.ContactAddress,
                        FooterText = result.Data.FooterText
                    };

                    return View(updateDto);
                }
            }

            TempData["Error"] = "获取系统设置失败";
            return View(new UpdateSystemSettingsDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统设置失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new UpdateSystemSettingsDto());
        }
    }

    /// <summary>
    /// 提交系统设置修改并将更改保存到后端。
    /// </summary>
    /// <param name="model">页面提交的系统设置数据。</param>
    /// <returns>保存成功后返回设置页，失败时重新显示表单和错误信息。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UpdateSystemSettingsDto model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync("/api/system/settings", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "系统设置更新成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<SystemSettingsDto>>(errorContent);
            
            if (errorResult?.Errors != null && errorResult.Errors.Any())
            {
                foreach (var error in errorResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorResult?.Message ?? "更新失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新系统设置失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        return View(model);
    }

    /// <summary>
    /// 查看数据库连接和运行状态信息。
    /// </summary>
    /// <returns>数据库状态页面。</returns>
    [HttpGet]
    public async Task<IActionResult> DatabaseStatus()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/system/database-status");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<DatabaseStatusDto>>(content);
                return View(result?.Data);
            }

            TempData["Error"] = "获取数据库状态失败";
            return View(new DatabaseStatusDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据库状态失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new DatabaseStatusDto());
        }
    }
}
