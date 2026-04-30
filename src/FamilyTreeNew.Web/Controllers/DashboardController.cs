using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 仪表盘控制器
/// 提供管理后台首页的统计数据和概览信息
/// </summary>
public class DashboardController : AdminControllerBase
{
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DashboardController> logger)
        : base(httpClientFactory, configuration)
    {
        _logger = logger;
    }

    /// <summary>
    /// 仪表盘首页
    /// 展示家谱数量、成员数量、相册数量、数据库状态和最近家谱列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var loginCheck = CheckLoginOrRedirect();
        if (loginCheck != null) return loginCheck;

        var viewModel = new DashboardViewModel();

        try
        {
            var client = GetApiClient();

            var familyTreesResponse = await client.GetAsync("/api/familytrees?pageSize=1");
            if (familyTreesResponse.IsSuccessStatusCode)
            {
                var content = await familyTreesResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyTreeDto>>>(content);
                viewModel.TotalFamilyTrees = result?.Data?.TotalCount ?? 0;

                var firstFamilyTreeId = result?.Data?.Items?.FirstOrDefault()?.Id;
                if (firstFamilyTreeId.HasValue)
                {
                    var membersResponse = await client.GetAsync($"/api/familymembers?familyTreeId={firstFamilyTreeId.Value}&pageSize=1");
                    if (membersResponse.IsSuccessStatusCode)
                    {
                        var membersContent = await membersResponse.Content.ReadAsStringAsync();
                        var membersResult = JsonConvert.DeserializeObject<ApiResponse<PagedResult<FamilyMemberDto>>>(membersContent);
                        viewModel.TotalMembers = membersResult?.Data?.TotalCount ?? 0;
                    }
                }
            }

            var albumsResponse = await client.GetAsync("/api/albums?pageSize=1");
            if (albumsResponse.IsSuccessStatusCode)
            {
                var content = await albumsResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<PagedResult<AlbumDto>>>(content);
                viewModel.TotalAlbums = result?.Data?.TotalCount ?? 0;
            }

            var dbStatusResponse = await client.GetAsync("/api/system/database-status");
            if (dbStatusResponse.IsSuccessStatusCode)
            {
                var content = await dbStatusResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<DatabaseStatusDto>>(content);
                viewModel.DatabaseStatus = result?.Data;
            }

            var recentTreesResponse = await client.GetAsync("/api/familytrees?pageSize=5");
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

        return View(viewModel);
    }
}

/// <summary>
/// 仪表盘视图模型
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// 家谱总数
    /// </summary>
    public int TotalFamilyTrees { get; set; }

    /// <summary>
    /// 成员总数
    /// </summary>
    public int TotalMembers { get; set; }

    /// <summary>
    /// 相册总数
    /// </summary>
    public int TotalAlbums { get; set; }

    /// <summary>
    /// 数据库状态信息
    /// </summary>
    public DatabaseStatusDto? DatabaseStatus { get; set; }

    /// <summary>
    /// 最近创建的家谱列表
    /// </summary>
    public List<FamilyTreeDto> RecentFamilyTrees { get; set; } = new();
}
