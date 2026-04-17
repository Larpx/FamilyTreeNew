using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
/// <summary>
/// 家谱管理控制器。
/// 负责后台家谱列表的查询、筛选和维护操作。
/// </summary>
public class FamilyTreeManagementController : AuthenticatedApiControllerBase
{
    private readonly ILogger<FamilyTreeManagementController> _logger;

    public FamilyTreeManagementController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<FamilyTreeManagementController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? keyword = null, bool? isEnabled = null)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var url = $"/api/familytrees?pageIndex={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(keyword))
            {
                url += $"&keyword={Uri.EscapeDataString(keyword)}";
            }
            if (isEnabled.HasValue)
            {
                url += $"&isEnabled={isEnabled.Value}";
            }

            var response = await client.GetAsync(url);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);

                ViewBag.Keyword = keyword;
                ViewBag.IsEnabled = isEnabled;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;

                return View(result?.Data);
            }

            SetErrorMessage("获取家谱列表失败");
            return View(new PagedResult<FamilyTreeDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱列表失败");
            SetErrorMessage("系统错误，请稍后重试");
            return View(new PagedResult<FamilyTreeDto>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        return View(new FamilyTreeCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FamilyTreeCreateDto model)
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
            var response = await client.PostAsJsonAsync("/api/familytrees", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("家谱创建成功");
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建家谱失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyTreeDto>>(content);

                if (result?.Data != null)
                {
                    var updateDto = new FamilyTreeUpdateDto
                    {
                        Name = result.Data.Name,
                        Description = result.Data.Description,
                        RequireVerification = result.Data.RequireVerification,
                        IsEnabled = result.Data.IsEnabled
                    };
                    ViewBag.FamilyTreeId = id;
                    return View(updateDto);
                }
            }

            SetErrorMessage("家谱不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱信息失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FamilyTreeUpdateDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        if (!ModelState.IsValid)
        {
            ViewBag.FamilyTreeId = id;
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync($"/api/familytrees/{id}", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("家谱更新成功");
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新家谱失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

        ViewBag.FamilyTreeId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/familytrees/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("家谱删除成功");
            }
            else
            {
                await SetResponseErrorMessageAsync(response, "删除失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除家谱失败");
            SetErrorMessage("系统错误，请稍后重试");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyTreeDto>>(content);
                return View(result?.Data);
            }

            SetErrorMessage("家谱不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱详情失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }
}
