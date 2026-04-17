using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class VerificationManagementController : AuthenticatedApiControllerBase
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
            var response = await client.GetAsync("/api/verificationquestions");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<VerificationQuestionDto>>>(content);
                return View(result?.Data ?? new List<VerificationQuestionDto>());
            }

            SetErrorMessage("获取验证问题列表失败");
            return View(new List<VerificationQuestionDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证问题列表失败");
            SetErrorMessage("系统错误，请稍后重试");
            return View(new List<VerificationQuestionDto>());
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

            return View(new CreateVerificationQuestionDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取创建验证问题页面失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVerificationQuestionDto model)
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
            var response = await client.PostAsJsonAsync("/api/verificationquestions", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("验证问题创建成功");
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建验证问题失败");
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
            var response = await client.GetAsync($"/api/verificationquestions/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<VerificationQuestionDto>(content);
                
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

            SetErrorMessage("验证问题不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证问题信息失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateVerificationQuestionDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        ViewBag.QuestionId = id;

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
            var response = await client.PutAsJsonAsync($"/api/verificationquestions/{id}", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("验证问题更新成功");
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新验证问题失败");
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
            var response = await client.DeleteAsync($"/api/verificationquestions/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("验证问题删除成功");
            }
            else
            {
                await SetResponseErrorMessageAsync(response, "删除失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除验证问题失败");
            SetErrorMessage("系统错误，请稍后重试");
        }

        return RedirectToAction(nameof(Index));
    }
}
