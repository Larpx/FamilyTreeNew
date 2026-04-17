using System.Security.Claims;
using FamilyTreeNew.Models.DTOs.Auth;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AdminController : AuthenticatedApiControllerBase
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AdminController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrWhiteSpace(token) && !IsTokenExpired(token) && (User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        if (!string.IsNullOrWhiteSpace(token) || (User.Identity?.IsAuthenticated ?? false))
        {
            await SignOutAndClearSessionAsync();
        }

        return View();
    }

    [AllowAnonymous]
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
                    HttpContext.Session.SetString("PermissionLevel", result.AdminInfo?.PermissionLevel.ToString() ?? "1");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.AdminInfo?.Id.ToString() ?? string.Empty),
                        new Claim(ClaimTypes.Name, result.AdminInfo?.Username ?? string.Empty),
                        new Claim("PermissionLevel", result.AdminInfo?.PermissionLevel.ToString() ?? "1")
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

                AddErrorMessage(result?.Message ?? "登录失败");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<LoginResponseDto>(errorContent);
                AddErrorMessage(errorResult?.Message ?? "登录失败，请检查用户名和密码");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录过程中发生错误");
            AddErrorMessage("系统错误，请稍后重试");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrWhiteSpace(token) && !IsTokenExpired(token))
            {
                var client = GetApiClient();
                await client.PostAsync("/api/auth/logout", null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出API调用失败");
        }

        await SignOutAndClearSessionAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? keyword = null)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        var permissionLevel = HttpContext.Session.GetString("PermissionLevel");
        if (permissionLevel != "3")
        {
            return Forbid();
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/admins?page={page}&pageSize={pageSize}&keyword={keyword}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<AdminDto>>>(content);
                return View(result?.Data);
            }

            SetErrorMessage("获取管理员列表失败");
            return View(new PagedResult<AdminDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取管理员列表失败");
            SetErrorMessage("系统错误，请稍后重试");
            return View(new PagedResult<AdminDto>());
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

        var permissionLevel = HttpContext.Session.GetString("PermissionLevel");
        if (permissionLevel != "3")
        {
            return Forbid();
        }

        return View(new CreateAdminDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAdminDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        var permissionLevel = HttpContext.Session.GetString("PermissionLevel");
        if (permissionLevel != "3")
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("/api/admins", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("管理员创建成功");
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "创建失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建管理员失败");
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

        var permissionLevel = HttpContext.Session.GetString("PermissionLevel");
        if (permissionLevel != "3")
        {
            return Forbid();
        }

        try
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/admins/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

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
                        PermissionLevel = result.Data.PermissionLevel,
                        IsEnabled = result.Data.IsEnabled
                    };
                    return View(updateDto);
                }
            }

            SetErrorMessage("管理员不存在");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取管理员信息失败");
            SetErrorMessage("系统错误，请稍后重试");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateAdminDto model)
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        var permissionLevel = HttpContext.Session.GetString("PermissionLevel");
        if (permissionLevel != "3")
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = GetApiClient();
            var response = await client.PutAsJsonAsync($"/api/admins/{id}", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "管理员更新成功";
                return RedirectToAction(nameof(Index));
            }

            await AddResponseErrorsAsync(response, "更新失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新管理员失败");
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

        var permissionLevel = HttpContext.Session.GetString("PermissionLevel");
        if (permissionLevel != "3")
        {
            return Forbid();
        }

        try
        {
            var client = GetApiClient();
            var response = await client.DeleteAsync($"/api/admins/{id}");

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("管理员删除成功");
            }
            else
            {
                await SetResponseErrorMessageAsync(response, "删除失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除管理员失败");
            SetErrorMessage("系统错误，请稍后重试");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        return View(new ChangePasswordRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto model)
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
            var response = await client.PostAsJsonAsync("/api/auth/change-password", model);

            var unauthorizedResult = await HandleUnauthorizedResponseAsync(response);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            if (response.IsSuccessStatusCode)
            {
                SetSuccessMessage("密码修改成功");
                return RedirectToAction("Index", "Dashboard");
            }

            await AddResponseErrorsAsync(response, "密码修改失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密码失败");
            AddErrorMessage("系统错误，请稍后重试");
        }

        return View(model);
    }
}

public class AdminDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int PermissionLevel { get; set; }
    public string? RealName { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsEnabled { get; set; }
}

public class CreateAdminDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int PermissionLevel { get; set; } = 1;
    public string? RealName { get; set; }
    public string? Email { get; set; }
}

public class UpdateAdminDto
{
    public string Username { get; set; } = string.Empty;
    public int PermissionLevel { get; set; }
    public string? RealName { get; set; }
    public string? Email { get; set; }
    public bool IsEnabled { get; set; }
}
