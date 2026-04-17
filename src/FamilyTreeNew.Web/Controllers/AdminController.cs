using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace FamilyTreeNew.Web.Controllers;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AdminController> logger)
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
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LoginResponseDto>(content);

                if (result != null && result.Success)
                {
                    HttpContext.Session.SetString("JwtToken", result.Token ?? string.Empty);
                    HttpContext.Session.SetString("AdminId", result.AdminInfo?.Id.ToString() ?? string.Empty);
                    HttpContext.Session.SetString("Username", result.AdminInfo?.Username ?? string.Empty);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.AdminInfo?.Id.ToString() ?? string.Empty),
                        new Claim(ClaimTypes.Name, result.AdminInfo?.Username ?? string.Empty)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError(string.Empty, result?.Message ?? "登录失败");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<LoginResponseDto>(errorContent);
                ModelState.AddModelError(string.Empty, errorResult?.Message ?? "登录失败，请检查用户名和密码");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生错误");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var client = GetApiClient();
            await client.PostAsync("/api/auth/logout", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出API调用失败");
        }

        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? keyword = null)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/admins?page={page}&pageSize={pageSize}&keyword={keyword}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<AdminDto>>>(content);
                return View(result?.Data);
            }

            TempData["Error"] = "获取管理员列表失败";
            return View(new PagedResult<AdminDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取管理员列表失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return View(new PagedResult<AdminDto>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login", "Admin");
        }

        return View(new CreateAdminDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAdminDto model)
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
            var response = await client.PostAsJsonAsync("/api/admins", model);


            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "管理员创建成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<object>>(errorContent);
            ModelState.AddModelError(string.Empty, errorResult?.Message ?? "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建管理员失败");
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
            var response = await client.GetAsync($"/api/admins/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AdminDto>>(content);
                
                if (result?.Data != null)
                {
                    var updateDto = new UpdateAdminDto
                    {
                        Username = result.Data.Username,
                        RealName = result.Data.RealName,
                        Email = result.Data.Email,
                        IsEnabled = result.Data.IsEnabled
                    };
                    return View(updateDto);
                }
            }

            TempData["Error"] = "管理员不存在";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取管理员信息失败");
            TempData["Error"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateAdminDto model)
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
            var response = await client.PutAsJsonAsync($"/api/admins/{id}", model);


            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "管理员更新成功";
                return RedirectToAction(nameof(Index));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<object>>(errorContent);
            ModelState.AddModelError(string.Empty, errorResult?.Message ?? "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新管理员失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

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
            var response = await client.DeleteAsync($"/api/admins/{id}");


            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "管理员删除成功";
            }
            else
            {
                TempData["Error"] = "删除失败";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除管理员失败");
            TempData["Error"] = "系统错误，请稍后重试";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login");
        }

        return View(new ChangePasswordRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("/api/auth/change-password", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "密码修改成功";
                return RedirectToAction("Index", "Dashboard");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ApiResponse<object>>(errorContent);
            ModelState.AddModelError(string.Empty, errorResult?.Message ?? "密码修改失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码失败");
            ModelState.AddModelError(string.Empty, "系统错误，请稍后重试");
        }

        return View(model);
    }
}
