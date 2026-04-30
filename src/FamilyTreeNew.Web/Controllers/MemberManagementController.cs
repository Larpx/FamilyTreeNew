using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 成员管理控制器
/// 提供家谱成员的增删改查功能
/// </summary>
public class MemberManagementController : AdminControllerBase
{
    private readonly ILogger<MemberManagementController> _logger;

    public MemberManagementController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MemberManagementController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// 成员列表页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, Guid? familyTreeId = null, string? keyword = null)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();

            var familyTreesResponse = await client.GetAsync($"/api/familytrees?pageSize=100&familyTreeId={familyTreeId}");
            if (familyTreesResponse.IsSuccessStatusCode)
            {
                var content = await familyTreesResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);
                ViewBag.FamilyTrees = result?.Data?.Items ?? new List<FamilyTreeDto>();
            }

            if (!familyTreeId.HasValue)
            {
                var firstTree = ViewBag.FamilyTrees as List<FamilyTreeDto>;
                if (firstTree != null && firstTree.Any())
                {
                    familyTreeId = firstTree.First().Id;
                }
            }

            if (familyTreeId.HasValue)
            {
                var url = $"/api/familymembers?familyTreeId={familyTreeId}&pageIndex={page}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(keyword))
                {
                    url += $"&keyword={Uri.EscapeDataString(keyword)}";
                }

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);

                    ViewBag.SelectedFamilyTreeId = familyTreeId;
                    ViewBag.Keyword = keyword;
                    ViewBag.PageIndex = page;
                    ViewBag.PageSize = pageSize;

                    return View(result?.Data);
                }
            }

            return View(new PagedResult<FamilyMemberDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成员列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new PagedResult<FamilyMemberDto>());
        }
    }

    /// <summary>
    /// 创建成员页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create(Guid familyTreeId)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{familyTreeId}/members?pageSize=1000");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);
                ViewBag.Parents = result?.Data?.Items ?? new List<FamilyMemberDto>();
            }

            var model = new FamilyMemberCreateDto
            {
                FamilyTreeId = familyTreeId
            };

            ViewBag.FamilyTreeId = familyTreeId;
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取创建成员页面失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index), new { familyTreeId });
        }
    }

    /// <summary>
    /// 提交创建成员
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FamilyMemberCreateDto model)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        if (!ModelState.IsValid)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"/api/familytrees/{model.FamilyTreeId}/members?pageSize=1000");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);
                    ViewBag.Parents = result?.Data?.Items ?? new List<FamilyMemberDto>();
                }
            }
            catch
            {
                ViewBag.Parents = new List<FamilyMemberDto>();
            }

            ViewBag.FamilyTreeId = model.FamilyTreeId;
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("/api/familymembers", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "成员创建成功";
                return RedirectToAction(nameof(Index), new { familyTreeId = model.FamilyTreeId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<FamilyMemberDto>>(errorContent);

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
            _logger.LogError(ex, "创建成员失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{model.FamilyTreeId}/members?pageSize=1000");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);
                ViewBag.Parents = result?.Data?.Items ?? new List<FamilyMemberDto>();
            }
        }
        catch
        {
            ViewBag.Parents = new List<FamilyMemberDto>();
        }

        ViewBag.FamilyTreeId = model.FamilyTreeId;
        return View(model);
    }

    /// <summary>
    /// 编辑成员页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familymembers/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyMemberDto>>(content);

                if (result?.Data != null)
                {
                    var parentsResponse = await client.GetAsync($"/api/familytrees/{result.Data.FamilyTreeId}/members?pageSize=1000");
                    if (parentsResponse.IsSuccessStatusCode)
                    {
                        var parentsContent = await parentsResponse.Content.ReadAsStringAsync();
                        var parentsResult = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(parentsContent);
                        ViewBag.Parents = parentsResult?.Data?.Items?.Where(m => m.Id != id).ToList() ?? new List<FamilyMemberDto>();
                    }

                    var updateDto = new FamilyMemberUpdateDto
                    {
                        ParentId = result.Data.ParentId,
                        Surname = result.Data.Surname,
                        FirstName = result.Data.FirstName,
                        Alias = result.Data.Alias,
                        Ranking = result.Data.Ranking,
                        GenerationName = result.Data.GenerationName,
                        BirthDateSolar = result.Data.BirthDateSolar,
                        BirthDateLunar = result.Data.BirthDateLunar,
                        Residence = result.Data.Residence,
                        Occupation = result.Data.Occupation,
                        PersonalInfo = result.Data.PersonalInfo,
                        Note = result.Data.Note,
                        IsDeceased = result.Data.IsDeceased,
                        DeathDateLunar = result.Data.DeathDateLunar,
                        DeathDateSolar = result.Data.DeathDateSolar,
                        Remarks = result.Data.Remarks
                    };

                    ViewBag.MemberId = id;
                    ViewBag.FamilyTreeId = result.Data.FamilyTreeId;
                    return View(updateDto);
                }
            }

            TempData["Error"] = "成员不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成员信息失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// 提交编辑成员
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FamilyMemberUpdateDto model)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        var familyTreeId = Request.Form["FamilyTreeId"];
        ViewBag.FamilyTreeId = familyTreeId;
        ViewBag.MemberId = id;

        if (!ModelState.IsValid)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"/api/familytrees/{familyTreeId}/members?pageSize=1000");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);
                    ViewBag.Parents = result?.Data?.Items?.Where(m => m.Id != id).ToList() ?? new List<FamilyMemberDto>();
                }
            }
            catch
            {
                ViewBag.Parents = new List<FamilyMemberDto>();
            }

            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync($"/api/familymembers/{id}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "成员更新成功";
                return RedirectToAction(nameof(Index), new { familyTreeId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<FamilyMemberDto>>(errorContent);

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
            _logger.LogError(ex, "更新成员失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{familyTreeId}/members?pageSize=1000");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);
                ViewBag.Parents = result?.Data?.Items?.Where(m => m.Id != id).ToList() ?? new List<FamilyMemberDto>();
            }
        }
        catch
        {
            ViewBag.Parents = new List<FamilyMemberDto>();
        }

        return View(model);
    }

    /// <summary>
    /// 删除成员
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid familyTreeId)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/familymembers/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "成员删除成功";
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
            _logger.LogError(ex, "删除成员失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index), new { familyTreeId });
    }

    /// <summary>
    /// 成员详情页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familymembers/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyMemberDto>>(content);
                return View(result?.Data);
            }

            TempData["Error"] = "成员不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成员详情失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }
}
