using System.Net.Http.Headers;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

public class FamilyTreeManagementController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FamilyTreeManagementController> _logger;

    public FamilyTreeManagementController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<FamilyTreeManagementController> logger)
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
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? keyword = null, bool? isEnabled = null)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
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

            TempData["Error"] = "获取家谱列表失败";
            return View(new PagedResult<FamilyTreeDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new PagedResult<FamilyTreeDto>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        return View(new FamilyTreeCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FamilyTreeCreateDto model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("/api/familytrees", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "家谱创建成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<FamilyTreeDto>>(errorContent);
            
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
            _logger.LogError(ex, "创建家谱失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{id}");

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

            TempData["Error"] = "家谱不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱信息失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FamilyTreeUpdateDto model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
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

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "家谱更新成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<FamilyTreeDto>>(errorContent);
            
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
            _logger.LogError(ex, "更新家谱失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        ViewBag.FamilyTreeId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/familytrees/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "家谱删除成功";
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
            _logger.LogError(ex, "删除家谱失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/familytrees/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<FamilyTreeDto>>(content);
                return View(result?.Data);
            }

            TempData["Error"] = "家谱不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱详情失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }
}
