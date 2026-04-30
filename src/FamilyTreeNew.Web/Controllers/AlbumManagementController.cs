using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 相册管理控制器
/// 提供相册的增删改查和照片管理功能
/// </summary>
public class AlbumManagementController : AdminControllerBase
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

    /// <summary>
    /// 相册列表页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, Guid? familyTreeId = null, string? keyword = null)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();

            var familyTreesResponse = await client.GetAsync("/api/familytrees?pageSize=100");
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

            TempData["Error"] = "获取相册列表失败";
            return View(new PagedResult<AlbumDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取相册列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new PagedResult<AlbumDto>());
        }
    }

    /// <summary>
    /// 创建相册页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create(Guid? familyTreeId)
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

            var model = new AlbumCreateDto
            {
                FamilyTreeId = familyTreeId ?? Guid.Empty
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取创建相册页面失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// 提交创建相册
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlbumCreateDto model)
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
            var response = await client.PostAsJsonAsync("/api/albums", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "相册创建成功";
                return RedirectToAction(nameof(Index), new { familyTreeId = model.FamilyTreeId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<AlbumDto>>(errorContent);

            if (errorResult?.Errors != null && errorResult.Errors.Any())
            {
                foreach (var error in errorResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorResult?.Message ?? "创建失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建相册失败");
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
    /// 编辑相册页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/albums/{id}");

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

            TempData["Error"] = "相册不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取相册信息失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// 提交编辑相册
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AlbumUpdateDto model)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

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

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "相册更新成功";
                return RedirectToAction(nameof(Index), new { familyTreeId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<AlbumDto>>(errorContent);

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
            _logger.LogError(ex, "更新相册失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        return View(model);
    }

    /// <summary>
    /// 删除相册
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
            var response = await client.DeleteAsync($"/api/albums/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "相册删除成功";
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
            _logger.LogError(ex, "删除相册失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 相册详情页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/albums/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AlbumDetailDto>>(content);
                return View(result?.Data);
            }

            TempData["Error"] = "相册不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取相册详情失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }
}
