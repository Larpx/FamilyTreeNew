using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 提供数据库备份列表查看、创建、恢复和删除操作。
/// </summary>
public class BackupController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BackupController> _logger;

    /// <summary>
    /// 初始化备份控制器。
    /// </summary>
    /// <param name="httpClientFactory">创建 API 请求客户端的工厂。</param>
    /// <param name="configuration">读取 API 地址等配置的配置对象。</param>
    /// <param name="logger">记录备份相关错误的日志器。</param>
    public BackupController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<BackupController> logger)
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
    /// 打开备份管理页面，并读取当前备份文件列表。
    /// </summary>
    /// <returns>备份列表页面。</returns>
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
            var response = await client.GetAsync("/api/system/backups");

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

    /// <summary>
    /// 创建新的系统备份文件。
    /// </summary>
    /// <returns>处理完成后返回备份列表页。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsync("/api/system/backup", null);

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

    /// <summary>
    /// 根据用户选择的备份文件执行恢复操作。
    /// </summary>
    /// <param name="fileName">要恢复的备份文件名。</param>
    /// <returns>处理完成后返回备份列表页。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(string fileName)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
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

    /// <summary>
    /// 删除指定的备份文件。
    /// </summary>
    /// <param name="fileName">要删除的备份文件名。</param>
    /// <returns>处理完成后返回备份列表页。</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string fileName)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
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
