using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
/// <summary>
/// Excel 导入控制器。
/// 负责上传成员导入文件、下载模板，以及展示导入结果与错误信息。
/// </summary>
public class ExcelImportController : AuthenticatedApiControllerBase
{
    /// <summary>
    /// TempData 中保存导入错误的键名。
    /// </summary>
    private const string ImportErrorsTempDataKey = "ImportErrors";

    /// <summary>
    /// TempData 中保存导入结果的键名。
    /// </summary>
    private const string ImportResultTempDataKey = "ImportResult";

    /// <summary>
    /// 日志记录器。
    /// 用于记录导入过程中的异常和状态。
    /// </summary>
    private readonly ILogger<ExcelImportController> _logger;

    public ExcelImportController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ExcelImportController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// Excel 导入首页。
    /// 用于展示可导入的家谱列表和导入入口。
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
            var response = await client.GetAsync("/api/familytrees?pageSize=100");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

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
            SetErrorMessage("系统错误，请稍后重试");
            return View();
        }
    }

    /// <summary>
    /// 上传 Excel 文件并执行导入。
    /// 会先检查文件类型，再把文件流发送到后端 API。
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(Guid familyTreeId, IFormFile excelFile)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        if (excelFile == null || excelFile.Length == 0)
        {
            SetErrorMessage("请选择要上传的Excel文件");
            return RedirectToAction(nameof(Index));
        }

        var extension = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
        {
            SetErrorMessage("请上传Excel文件（.xlsx或.xls格式）");
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

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<ExcelImportResultDto>>(responseContent);

                if (result?.Data != null)
                {
                    var importResult = result.Data;

                    if (importResult.Success)
                    {
                        SetSuccessMessage($"导入成功！共导入 {importResult.ImportedCount} 条记录");
                    }
                    else
                    {
                        SetErrorMessage(importResult.Message);
                        SetImportErrors(importResult.Errors);
                    }

                    SetImportResult(importResult);
                }
            }
            else
            {
                await SetResponseErrorMessageAsync(response, "导入失败");
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<ApiResponse<ExcelImportResultDto>>(errorContent);
                SetImportErrors(errorResult?.Data?.Errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel导入失败");
            SetErrorMessage("系统错误，请稍后重试");
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 下载 Excel 导入模板。
    /// 供管理员按模板填写成员数据后再导入。
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DownloadTemplate()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/familytrees/template");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "成员导入模板.xlsx");
            }

            SetErrorMessage("下载模板失败");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下载模板失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    private void SetImportErrors(IEnumerable<ExcelImportErrorDto>? errors)
    {
        if (errors == null || !errors.Any())
        {
            return;
        }

        TempData[ImportErrorsTempDataKey] = JsonConvert.SerializeObject(errors);
    }

    /// <summary>
    /// 将导入结果保存到 `TempData`。
    /// 页面重定向后仍可读取并展示导入统计信息。
    /// </summary>
    private void SetImportResult(ExcelImportResultDto importResult)
    {
        TempData[ImportResultTempDataKey] = JsonConvert.SerializeObject(importResult);
    }
}
