using System.Net.Http.Headers;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

public class ExcelImportController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExcelImportController> _logger;

    public ExcelImportController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ExcelImportController> logger)
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
            var response = await client.GetAsync("/api/familytrees?pageSize=100");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);
                ViewBag.FamilyTrees = result?.Data?.Items ?? new List<FamilyTreeDto>();
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(Guid familyTreeId, IFormFile excelFile)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        if (excelFile == null || excelFile.Length == 0)
        {
            TempData["Error"] = "请选择要上传的Excel文件";
            return RedirectToAction(nameof(Index));
        }

        var extension = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "请上传Excel文件（.xlsx或.xls格式）";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var client = GetApiClient();
            
            using var content = new MultipartFormDataContent();
            using var fileStream = excelFile.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(streamContent, "file", excelFile.FileName);

            var response = await client.PostAsync($"/api/familytrees/{familyTreeId}/import", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<ExcelImportResultDto>>(responseContent);
                
                if (result?.Data != null)
                {
                    var importResult = result.Data;
                    
                    if (importResult.Success)
                    {
                        TempData["Success"] = $"导入成功！共导入 {importResult.ImportedCount} 条记录";
                    }
                    else
                    {
                        TempData["Error"] = importResult.Message;
                        if (importResult.Errors != null && importResult.Errors.Any())
                        {
                            TempData["ImportErrors"] = JsonConvert.SerializeObject(importResult.Errors);
                        }
                    }
                    
                    TempData["ImportResult"] = JsonConvert.SerializeObject(importResult);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<ApiResponse<ExcelImportResultDto>>(errorContent);
                TempData["Error"] = errorResult?.Message ?? "导入失败";
                
                if (errorResult?.Data?.Errors != null && errorResult.Data.Errors.Any())
                {
                    TempData["ImportErrors"] = JsonConvert.SerializeObject(errorResult.Data.Errors);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel导入失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DownloadTemplate()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/familytrees/template");

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "成员导入模板.xlsx");
            }

            TempData["Error"] = "下载模板失败";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下载模板失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }
}
