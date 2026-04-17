using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class MemberManagementController : AuthenticatedApiControllerBase
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

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, Guid? familyTreeId = null, string? keyword = null)
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

                unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
                if (unauthorizedResult != null)
                {
                    return unauthorizedResult;
                }

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
            SetErrorMessage("系统错误，请稍后重试");
            return View(new PagedResult<FamilyMemberDto>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid familyTreeId)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{familyTreeId}/members?pageSize=1000");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

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
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index), new { familyTreeId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FamilyMemberCreateDto model)
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
                var response = await client.GetAsync($"/api/familytrees/{model.FamilyTreeId}/members?pageSize=1000");

                var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
                if (unauthorizedResult != null)
                {
                    return unauthorizedResult;
                }

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

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("成员创建成功");
                return RedirectToAction(nameof(Index), new { familyTreeId = model.FamilyTreeId });
            }

            await AddResponseErrorsAsync(response, "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建成员失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{model.FamilyTreeId}/members?pageSize=1000");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

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
            var response = await client.GetAsync($"/api/familymembers/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyMemberDto>>(content);

                if (result?.Data != null)
                {
                    var parentsResponse = await client.GetAsync($"/api/familytrees/{result.Data.FamilyTreeId}/members?pageSize=1000");
                    unauthorizedResult = await HandleUnauthorizedResponseAsync(parentsResponse);
                    if (unauthorizedResult != null)
                    {
                        return unauthorizedResult;
                    }
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

            SetErrorMessage("成员不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成员信息失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FamilyMemberUpdateDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        var familyTreeId = Request.Form["FamilyTreeId"];
        ViewBag.FamilyTreeId = familyTreeId;
        ViewBag.MemberId = id;

        if (!ModelState.IsValid)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"/api/familytrees/{familyTreeId}/members?pageSize=1000");

                var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
                if (unauthorizedResult != null)
                {
                    return unauthorizedResult;
                }

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

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("成员更新成功");
                return RedirectToAction(nameof(Index), new { familyTreeId });
            }

            await AddResponseErrorsAsync(response, "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新成员失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{familyTreeId}/members?pageSize=1000");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid familyTreeId)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/familymembers/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("成员删除成功");
            }
            else
            {
                await SetResponseErrorMessageAsync(response, "删除失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除成员失败");
            SetErrorMessage("系统错误，请稍后重试");
        }

        return RedirectToAction(nameof(Index), new { familyTreeId });
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
            var response = await client.GetAsync($"/api/familymembers/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyMemberDto>>(content);
                return View(result?.Data);
            }

            SetErrorMessage("成员不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成员详情失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }
}
