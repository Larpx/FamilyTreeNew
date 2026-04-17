using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
/// <summary>
/// 后台仪表盘控制器。
/// 负责汇总家谱、成员、相册和数据库状态等统计信息，让管理员快速查看系统概况。
/// </summary>
public class DashboardController : AuthenticatedApiControllerBase
{
    private readonly ILogger<DashboardController> _logger;

    /// <summary>
    /// 构造函数。
    /// 用于注入调用 API 和记录日志所需的对象。
    /// </summary>
    public DashboardController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DashboardController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// 仪表盘首页。
    /// 负责收集家谱、成员、相册和数据库状态的统计信息并展示给管理员。
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var authResult = await EnsureAuthenticatedAsync();
        if (authResult != null)
        {
            return authResult;
        }

        var viewModel = new DashboardViewModel();

        try
        {
            var client = GetApiClient();

            var familyTreesResponse = await client.GetAsync("/api/familytrees?pageSize=1");
            var unauthorizedResult = await HandleUnauthorizedResponseAsync(familyTreesResponse);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }
            if (familyTreesResponse.IsSuccessStatusCode)
            {
                var content = await familyTreesResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);
                viewModel.TotalFamilyTrees = result?.Data?.TotalCount ?? 0;
            }

            var membersResponse = await client.GetAsync("/api/familymembers?pageSize=1");
            unauthorizedResult = await HandleUnauthorizedResponseAsync(membersResponse);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }
            if (membersResponse.IsSuccessStatusCode)
            {
                var content = await membersResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(content);
                viewModel.TotalMembers = result?.Data?.TotalCount ?? 0;
            }

            var albumsResponse = await client.GetAsync("/api/albums?pageSize=1");
            unauthorizedResult = await HandleUnauthorizedResponseAsync(albumsResponse);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }
            if (albumsResponse.IsSuccessStatusCode)
            {
                var content = await albumsResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<AlbumDto>>>(content);
                viewModel.TotalAlbums = result?.Data?.TotalCount ?? 0;
            }

            var dbStatusResponse = await client.GetAsync("/api/system/database-status");
            unauthorizedResult = await HandleUnauthorizedResponseAsync(dbStatusResponse);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }
            if (dbStatusResponse.IsSuccessStatusCode)
            {
                var content = await dbStatusResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<DatabaseStatusDto>>(content);
                viewModel.DatabaseStatus = result?.Data;
            }

            var recentTreesResponse = await client.GetAsync("/api/familytrees?pageSize=5");
            unauthorizedResult = await HandleUnauthorizedResponseAsync(recentTreesResponse);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }
            if (recentTreesResponse.IsSuccessStatusCode)
            {
                var content = await recentTreesResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);
                viewModel.RecentFamilyTrees = result?.Data?.Items ?? new List<FamilyTreeDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取仪表盘数据失败");
        }

        ViewBag.Username = HttpContext.Session.GetString("Username");
        ViewBag.PermissionLevel = HttpContext.Session.GetString("PermissionLevel");

        return View(viewModel);
    }
}

/// <summary>
/// 仪表盘页面的数据模型。
/// 用来把多个统计结果一次性传给 Razor 视图显示。
/// </summary>
public class DashboardViewModel
{
    public int TotalFamilyTrees { get; set; }
    public int TotalMembers { get; set; }
    public int TotalAlbums { get; set; }
    public DatabaseStatusDto? DatabaseStatus { get; set; }
    public List<FamilyTreeDto> RecentFamilyTrees { get; set; } = new();
}
