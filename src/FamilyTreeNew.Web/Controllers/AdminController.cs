using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
/// <summary>
/// 后台管理员管理控制器。
/// 负责管理员登录、退出、管理员列表维护，以及当前账号的密码修改。
/// </summary>
public class AdminController : AuthenticatedApiControllerBase
{
    private readonly ILogger<AdminController> _logger;

    /// <summary>
    /// 构造函数。
    /// 通过依赖注入拿到调用 API 所需的 `HttpClientFactory`、配置和日志记录器。
    /// </summary>
    public AdminController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AdminController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// 显示登录页面。
    /// 如果当前用户已经登录且会话有效，就直接跳转到仪表盘；否则返回登录视图。
    /// </summary>
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

    /// <summary>
    /// 处理登录提交。
    /// 它会把用户名和密码发送到后端 API，成功后把 JWT 和 Cookie 登录态保存到会话中。
    /// </summary>
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

    /// <summary>
    /// 处理管理员退出登录。
    /// 如果当前会话里有未过期的 JWT，就先调用后端登出接口，再清理本地会话和 Cookie。
    /// </summary>
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

    /// <summary>
    /// 显示管理员列表。
    /// 只有最高权限管理员可以访问，并且支持分页和关键字搜索。
    /// </summary>
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

    /// <summary>
    /// 显示新增管理员页面。
    /// 页面仅允许最高权限管理员进入，用于创建新的后台账号。
    /// </summary>
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

    /// <summary>
    /// 提交新增管理员请求。
    /// 如果表单验证通过，就把创建信息提交给后端 API。
    /// </summary>
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

    /// <summary>
    /// 显示编辑管理员页面。
    /// 先从后端 API 读取管理员详情，再把数据填入编辑表单。
    /// </summary>
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

    /// <summary>
    /// 提交管理员编辑请求。
    /// 用于保存用户名、权限级别、姓名、邮箱和启用状态等修改内容。
    /// </summary>
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

    /// <summary>
    /// 删除指定管理员。
    /// 该操作仅允许最高权限管理员执行，删除后会刷新列表页。
    /// </summary>
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

    /// <summary>
    /// 显示修改当前登录账号密码的页面。
    /// 页面只负责输入旧密码和新密码，不会暴露管理员的其他信息。
    /// </summary>
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

    /// <summary>
    /// 提交密码修改请求。
    /// 成功后会提示用户重新登录或返回后台首页继续使用系统。
    /// </summary>
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

/// <summary>
/// 管理员列表中的展示模型。
/// 页面会使用它显示管理员的基本资料、权限级别和账户状态。
/// </summary>
public class AdminDto
{
    /// <summary>
    /// 管理员唯一标识。
    /// 前端在编辑、删除或查看详情时会使用这个值定位记录。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 管理员登录用户名。
    /// 这是后台登录时最常用的身份标识。
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 权限级别。
    /// 数值越大通常代表权限越高。
    /// </summary>
    public int PermissionLevel { get; set; }

    /// <summary>
    /// 管理员真实姓名。
    /// 这个字段可用于页面展示，帮助管理员更容易识别账号所属人员。
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 管理员联系邮箱。
    /// 用于通知、找回密码或内部沟通。
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 账户创建时间。
    /// 方便管理员查看账号是何时创建的。
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最近一次登录时间。
    /// 如果从未登录过，这个值可能为空。
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 账号是否启用。
    /// 当值为 false 时，账号通常不能登录系统。
    /// </summary>
    public bool IsEnabled { get; set; }
}

/// <summary>
/// 创建管理员时提交的数据模型。
/// 包含用户名、初始密码以及可选的姓名和邮箱信息。
/// </summary>
public class CreateAdminDto
{
    /// <summary>
    /// 要创建的管理员用户名。
    /// 这个值通常要求唯一，避免不同账号使用相同登录名。
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 初始密码。
    /// 用户首次登录时将使用该密码，之后建议立即修改。
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 初始权限级别。
    /// 默认值为 1，具体含义由系统权限规则决定。
    /// </summary>
    public int PermissionLevel { get; set; } = 1;

    /// <summary>
    /// 管理员真实姓名。
    /// 可为空，主要用于提升后台可读性。
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 管理员联系邮箱。
    /// 可选填写，用于通知或联系。
    /// </summary>
    public string? Email { get; set; }
}

/// <summary>
/// 编辑管理员时提交的数据模型。
/// 与创建模型相比，这里不再包含密码字段，而是允许修改启用状态。
/// </summary>
public class UpdateAdminDto
{
    /// <summary>
    /// 管理员用户名。
    /// 编辑时如果允许修改用户名，这里会保存新的登录名。
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 权限级别。
    /// 用于控制该账号能访问哪些后台功能。
    /// </summary>
    public int PermissionLevel { get; set; }

    /// <summary>
    /// 管理员真实姓名。
    /// 用于后台展示和账号识别。
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 管理员联系邮箱。
    /// 便于后续联系或发送通知。
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 账号是否启用。
    /// 如果禁用，管理员将无法继续登录。
    /// </summary>
    public bool IsEnabled { get; set; }
}
