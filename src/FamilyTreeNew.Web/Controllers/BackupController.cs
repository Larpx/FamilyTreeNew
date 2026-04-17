using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class BackupController : AuthenticatedApiControllerBase
{
    private readonly ILogger<BackupController> _logger;

    public BackupController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<BackupController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

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
            var response = await client.GetAsync("/api/system/backups");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<BackupListDto>>(content);
                return View(result?.Data);
            }

            TempData["Error"] = "获取备份列表失败";
            return View(new BackupListDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取备份列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new BackupListDto());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsync("/api/system/backup", null);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<BackupDto>>(content);
                
                if (result?.Data?.IsSuccess == true)
                {
                    TempData["Success"] = "备份创建成功";
                }
                else
                {
                    TempData["Error"] = result?.Data?.ErrorMessage ?? "备份创建失败";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<ApiResponse<BackupDto>>(errorContent);
                TempData["Error"] = errorResult?.Message ?? "备份创建失败";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建备份失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(string fileName)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            TempData["Error"] = "备份文件名不能为空";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var client = GetApiClient();
            var request = new RestoreRequestDto { FileName = fileName };
            var response = await client.PostAsJsonAsync("/api/system/restore", request);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<RestoreDto>>(content);
                
                if (result?.Data?.IsSuccess == true)
                {
                    TempData["Success"] = "备份恢复成功";
                }
                else
                {
                    TempData["Error"] = result?.Data?.ErrorMessage ?? "备份恢复失败";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<ApiResponse<RestoreDto>>(errorContent);
                TempData["Error"] = errorResult?.Message ?? "备份恢复失败";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "恢复备份失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string fileName)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            TempData["Error"] = "备份文件名不能为空";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/system/backups/{Uri.EscapeDataString(fileName)}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "备份删除成功";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<ApiResponse>(errorContent);
                TempData["Error"] = errorResult?.Message ?? "删除失败";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除备份失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }
}
