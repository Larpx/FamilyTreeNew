using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Web.Models;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 首页控制器
/// 展示家谱列表和平台介绍信息
/// </summary>
public class HomeController : Controller
{
    private readonly IFamilyTreeService _familyTreeService;

    public HomeController(IFamilyTreeService familyTreeService)
    {
        _familyTreeService = familyTreeService;
    }

    /// <summary>
    /// 首页
    /// 展示最新家谱列表和平台数据概览
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var result = await _familyTreeService.GetPagedAsync(new FamilyTreeQueryDto { PageSize = 100 });
        var familyTrees = result?.Items ?? new List<FamilyTreeDto>();
        return View(familyTrees);
    }

    /// <summary>
    /// 隐私政策页面
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
