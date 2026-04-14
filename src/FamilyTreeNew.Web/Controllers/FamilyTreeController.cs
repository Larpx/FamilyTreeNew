using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 家谱展示控制器
/// 
/// 负责家谱的各种展示视图，包括：
/// - 家谱列表页（Index）
/// - 家谱详情页（Detail）
/// - 吊线图（LineageChart）
/// - 世系表格（TableChart）
/// - 树形结构（TreeView）
/// - 成员列表（TableView）
/// - 时间轴（Timeline）
/// - 关系图谱（RelationGraph）
/// - 成员详情API（GetMemberDetail/GetMembers）
/// </summary>
public class FamilyTreeController : Controller
{
    private readonly IFamilyTreeService _familyTreeService;
    private readonly IFamilyMemberService _memberService;
    private readonly IVerificationService _verificationService;
    private readonly IConfiguration _configuration;

    public FamilyTreeController(
        IFamilyTreeService familyTreeService,
        IFamilyMemberService memberService,
        IVerificationService verificationService,
        IConfiguration configuration)
    {
        _familyTreeService = familyTreeService;
        _memberService = memberService;
        _verificationService = verificationService;
        _configuration = configuration;
    }

    /// <summary>
    /// 家谱列表页
    /// 展示所有家谱的列表，每页最多100条。
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var query = new FamilyTreeQueryDto { PageSize = 100 };
        var result = await _familyTreeService.GetPagedAsync(query);
        return View(result.Items);
    }

    /// <summary>
    /// 家谱详情页
    /// 
    /// 展示家谱的详细信息，包含标签页导航用于切换不同图表视图。
    /// 如果家谱需要验证访问权限，会检查访问令牌。
    /// </summary>
    /// <param name="id">家谱ID</param>
    public async Task<IActionResult> Detail(Guid id)
    {
        var familyTree = await _familyTreeService.GetByIdAsync(id);
        if (familyTree == null)
        {
            return NotFound();
        }

        if (familyTree.RequireVerification)
        {
            string? token = Request.Cookies[$"ft_access_{id}"];
            bool hasAccess = false;

            if (!string.IsNullOrEmpty(token))
            {
                hasAccess = _verificationService.ValidateAccessToken(token, id);
            }

            if (!hasAccess)
            {
                return RedirectToAction("Verify", "Verification", new { familyTreeId = id });
            }
        }

        var members = await _memberService.GetByFamilyTreeIdAsync(id);
        ViewBag.FamilyTree = familyTree;
        ViewBag.Members = members;
        return View();
    }

    /// <summary>
    /// 通用图表视图入口
    /// 根据type参数渲染不同的图表类型。
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="type">图表类型（lineage/table/tree/timeline/graph）</param>
    public async Task<IActionResult> Chart(Guid id, string type = "lineage")
    {
        var familyTree = await _familyTreeService.GetByIdAsync(id);
        if (familyTree == null)
        {
            return NotFound();
        }

        var members = await _memberService.GetByFamilyTreeIdAsync(id);
        ViewBag.FamilyTree = familyTree;
        ViewBag.Members = members;
        ViewBag.ChartType = type;
        return View();
    }

    /// <summary>
    /// 加载家谱基础数据到ViewBag
    /// 
    /// 提取公共逻辑：获取家谱信息和成员列表，设置到ViewBag中。
    /// 如果家谱不存在则返回NotFound。
    /// 多个图表视图Action共用此方法避免重复代码。
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>家谱DTO对象，不存在时返回null</returns>
    private async Task<FamilyTreeDto?> LoadFamilyTreeData(Guid id)
    {
        var familyTree = await _familyTreeService.GetByIdAsync(id);
        if (familyTree == null)
        {
            return null;
        }

        var members = await _memberService.GetByFamilyTreeIdAsync(id);
        ViewBag.FamilyTree = familyTree;
        ViewBag.Members = members;
        return familyTree;
    }

    /// <summary>
    /// 吊线图视图
    /// 使用D3.js树形布局展示家谱的吊线图。
    /// </summary>
    /// <param name="id">家谱ID</param>
    public async Task<IActionResult> LineageChart(Guid id)
    {
        if (await LoadFamilyTreeData(id) == null) return NotFound();
        return View();
    }

    /// <summary>
    /// 世系表格视图
    /// 按世代分组展示家谱成员，支持横向/纵向方向切换。
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="direction">展示方向（horizontal/vertical），默认横向</param>
    public async Task<IActionResult> TableChart(Guid id, string direction = "horizontal")
    {
        if (await LoadFamilyTreeData(id) == null) return NotFound();
        ViewBag.Direction = direction;
        return View();
    }

    /// <summary>
    /// 树形结构视图
    /// 以可折叠的树形列表展示家谱成员的层级关系。
    /// </summary>
    /// <param name="id">家谱ID</param>
    public async Task<IActionResult> TreeView(Guid id)
    {
        if (await LoadFamilyTreeData(id) == null) return NotFound();
        return View();
    }

    /// <summary>
    /// 成员列表视图
    /// 以分页表格形式展示家谱成员，支持搜索和排序。
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="page">页码，默认第1页</param>
    /// <param name="pageSize">每页条数，默认20条</param>
    /// <param name="keyword">搜索关键词，可选</param>
    public async Task<IActionResult> TableView(Guid id, int page = 1, int pageSize = 20, string? keyword = null)
    {
        var familyTree = await _familyTreeService.GetByIdAsync(id);
        if (familyTree == null)
        {
            return NotFound();
        }

        var query = new FamilyMemberQueryDto
        {
            FamilyTreeId = id,
            PageIndex = page,
            PageSize = pageSize,
            Keyword = keyword
        };

        var result = await _memberService.GetPagedAsync(query);
        ViewBag.FamilyTree = familyTree;
        ViewBag.Keyword = keyword;
        return View(result);
    }

    /// <summary>
    /// 时间轴视图
    /// 按时间顺序展示家谱成员的出生和逝世事件。
    /// </summary>
    /// <param name="id">家谱ID</param>
    public async Task<IActionResult> Timeline(Guid id)
    {
        if (await LoadFamilyTreeData(id) == null) return NotFound();
        return View();
    }

    /// <summary>
    /// 关系图谱视图
    /// 使用D3.js力导向图展示家谱成员的关系网络。
    /// </summary>
    /// <param name="id">家谱ID</param>
    public async Task<IActionResult> RelationGraph(Guid id)
    {
        if (await LoadFamilyTreeData(id) == null) return NotFound();
        return View();
    }

    /// <summary>
    /// 获取成员详情API
    /// 
    /// 通过AJAX请求获取单个成员的详细信息，返回JSON格式数据。
    /// 用于图表页面中点击节点时弹出成员详情弹窗。
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>JSON格式的成员数据</returns>
    [HttpGet]
    public async Task<IActionResult> GetMemberDetail(Guid id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
        {
            return Json(new { success = false, message = "成员不存在" });
        }
        return Json(new { success = true, data = member });
    }

    /// <summary>
    /// 获取家谱所有成员API
    /// 
    /// 返回指定家谱的所有成员列表，用于图表数据加载。
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>JSON格式的成员列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetMembers(Guid familyTreeId)
    {
        var members = await _memberService.GetByFamilyTreeIdAsync(familyTreeId);
        return Json(new { success = true, data = members });
    }
}
