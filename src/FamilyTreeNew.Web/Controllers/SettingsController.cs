using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
/// <summary>
/// 系统设置管理控制器。
/// 负责查看和修改网站设置，以及查看数据库状态。
/// </summary>
public class SettingsController : AuthenticatedApiControllerBase
{
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SettingsController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// 显示系统设置页面。
    /// 从后端 API 读取当前设置并填充到编辑表单中。
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/system/settings");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

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

            SetErrorMessage("获取系统设置失败");
            return View(new UpdateSystemSettingsDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统设置失败");
            SetErrorMessage("系统错误，请稍后重试");
            return View(new UpdateSystemSettingsDto());
        }
    }

    /// <summary>
    /// 提交系统设置更新。
    /// 当表单验证通过时，会把新的设置保存到后端。
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UpdateSystemSettingsDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync("/api/system/settings", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("系统设置更新成功");
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新系统设置失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

        return View(model);
    }

    /// <summary>
    /// 数据库状态页面。
    /// 用于展示数据库是否连通、表数量和统计信息。
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DatabaseStatus()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/system/database-status");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<DatabaseStatusDto>>(content);
                return View(result?.Data);
            }

            SetErrorMessage("获取数据库状态失败");
            return View(new DatabaseStatusDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据库状态失败");
            SetErrorMessage("系统错误，请稍后重试");
            return View(new DatabaseStatusDto());
        }
    }
}
