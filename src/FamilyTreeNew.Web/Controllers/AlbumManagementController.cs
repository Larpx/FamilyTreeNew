using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
/// <summary>
/// 相册管理控制器。
/// 用于在后台查看、筛选和维护相册数据。
/// </summary>
public class AlbumManagementController : AuthenticatedApiControllerBase
{
    private readonly ILogger<AlbumManagementController> _logger;

    public AlbumManagementController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AlbumManagementController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, Guid? familyTreeId = null, string? keyword = null)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();

            var familyTreesResponse = await client.GetAsync("/api/familytrees?pageSize=100");
            var unauthorizedResult = await HandleUnauthorizedResponseAsync(familyTreesResponse);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }
            if (familyTreesResponse.IsSuccessStatusCode)
            {
                var content = await familyTreesResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);
                ViewBag.FamilyTrees = result?.Data?.Items ?? new List<FamilyTreeDto>();
            }

            var url = $"/api/albums?pageIndex={page}&pageSize={pageSize}";
            if (familyTreeId.HasValue)
            {
                url += $"&familyTreeId={familyTreeId.Value}";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                url += $"&keyword={Uri.EscapeDataString(keyword)}";
            }

            var response = await client.GetAsync(url);

            unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<AlbumDto>>>(content);

                ViewBag.SelectedFamilyTreeId = familyTreeId;
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;

                return View(result?.Data);
            }

            SetErrorMessage("获取相册列表失败");
            return View(new PagedResult<AlbumDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取相册列表失败");
            SetErrorMessage("系统错误，请稍后重试");
            return View(new PagedResult<AlbumDto>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? familyTreeId)
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

            var model = new AlbumCreateDto
            {
                FamilyTreeId = familyTreeId ?? Guid.Empty
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取创建相册页面失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlbumCreateDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        if (!ModelState.IsValid)
        {
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
            var response = await client.PostAsJsonAsync("/api/albums", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("相册创建成功");
                return RedirectToAction(nameof(Index), new { familyTreeId = model.FamilyTreeId });
            }

            await AddResponseErrorsAsync(response, "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建相册失败");
            AddErrorMessage("系统错误，请稍后重试");
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
        }
        catch
        {
            ViewBag.FamilyTrees = new List<FamilyTreeDto>();
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
            var response = await client.GetAsync($"/api/albums/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AlbumDetailDto>>(content);

                if (result?.Data != null)
                {
                    var updateDto = new AlbumUpdateDto
                    {
                        Name = result.Data.Name,
                        Description = result.Data.Description
                    };

                    ViewBag.AlbumId = id;
                    ViewBag.FamilyTreeId = result.Data.FamilyTreeId;
                    ViewBag.Photos = result.Data.Photos;
                    return View(updateDto);
                }
            }

            SetErrorMessage("相册不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取相册信息失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AlbumUpdateDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        var familyTreeId = Request.Form["FamilyTreeId"];
        ViewBag.AlbumId = id;
        ViewBag.FamilyTreeId = familyTreeId;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync($"/api/albums/{id}", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("相册更新成功");
                return RedirectToAction(nameof(Index), new { familyTreeId });
            }

            await AddResponseErrorsAsync(response, "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新相册失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

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
            var response = await client.DeleteAsync($"/api/albums/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("相册删除成功");
            }
            else
            {
                await SetResponseErrorMessageAsync(response, "删除失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除相册失败");
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
            var response = await client.GetAsync($"/api/albums/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AlbumDetailDto>>(content);
                return View(result?.Data);
            }

            SetErrorMessage("相册不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取相册详情失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }
}
