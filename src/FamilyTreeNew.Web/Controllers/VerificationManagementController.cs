using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 验证问题管理控制器
/// 提供验证问题的增删改查功能
/// </summary>
public class VerificationManagementController : AdminControllerBase
{
    private readonly ILogger<VerificationManagementController> _logger;

    public VerificationManagementController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<VerificationManagementController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// 验证问题列表页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync("/api/verificationquestions");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<VerificationQuestionDto>>>(content);
                return View(result?.Data ?? new List<VerificationQuestionDto>());
            }

            TempData["Error"] = "获取验证问题列表失败";
            return View(new List<VerificationQuestionDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证问题列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new List<VerificationQuestionDto>());
        }
    }

    /// <summary>
    /// 创建验证问题页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

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

            return View(new CreateVerificationQuestionDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取创建验证问题页面失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// 提交创建验证问题
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVerificationQuestionDto model)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        if (!ModelState.IsValid)
        {
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
            }
            catch
            {
                ViewBag.FamilyTrees = new List<FamilyTreeDto>();
            }

            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("/api/verificationquestions", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "验证问题创建成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse>(errorContent);
            ModelState.AddModelError(string.Empty, errorResult?.Message ?? "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建验证问题失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
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
        }
        catch
        {
            ViewBag.FamilyTrees = new List<FamilyTreeDto>();
        }

        return View(model);
    }

    /// <summary>
    /// 编辑验证问题页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/verificationquestions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResponse<VerificationQuestionDto>>(content);
                var result = apiResult?.Data;

                if (result != null)
                {
                    var updateDto = new UpdateVerificationQuestionDto
                    {
                        Question = result.Question,
                        Order = result.Order,
                        IsEnabled = result.IsEnabled
                    };

                    var familyTreesResponse = await client.GetAsync("/api/familytrees?pageSize=100");
                    if (familyTreesResponse.IsSuccessStatusCode)
                    {
                        var familyTreesContent = await familyTreesResponse.Content.ReadAsStringAsync();
                        var familyTreesResult = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(familyTreesContent);
                        ViewBag.FamilyTrees = familyTreesResult?.Data?.Items ?? new List<FamilyTreeDto>();
                    }
                    ViewBag.FamilyTreeId = result.FamilyTreeId;
                    ViewBag.QuestionId = id;
                    return View(updateDto);
                }
            }

            TempData["Error"] = "验证问题不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证问题信息失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// 提交编辑验证问题
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateVerificationQuestionDto model)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        ViewBag.QuestionId = id;

        if (!ModelState.IsValid)
        {
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
            }
            catch
            {
                ViewBag.FamilyTrees = new List<FamilyTreeDto>();
            }

            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync($"/api/verificationquestions/{id}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "验证问题更新成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse>(errorContent);
            ModelState.AddModelError(string.Empty, errorResult?.Message ?? "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新验证问题失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
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
        }
        catch
        {
            ViewBag.FamilyTrees = new List<FamilyTreeDto>();
        }

        return View(model);
    }

    /// <summary>
    /// 删除验证问题
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/verificationquestions/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "验证问题删除成功";
            }
            else
            {
                TempData["Error"] = "删除失败";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除验证问题失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }
}
