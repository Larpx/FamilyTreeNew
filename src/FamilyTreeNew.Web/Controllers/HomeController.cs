using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FamilyTreeNew.Web.Controllers;

/// <summary>
/// 网站首页控制器。
/// 负责展示公开页面，例如首页、隐私说明，以及统一错误页。
/// </summary>
public class HomeController : Controller
{
    private readonly IFamilyService _familyService;

    public HomeController(IFamilyService familyService)
    {
        _familyService = familyService;
    }

    public async Task<IActionResult> Index()
    {
        var families = await _familyService.GetAllFamiliesAsync();
        return View(families);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = 500,
            Title = "服务器开小差了",
            Message = exceptionFeature?.Error is not null
                ? "服务器处理请求时出现异常，请稍后重试。"
                : "服务器暂时无法完成当前请求，请稍后再试。"
        });
    }

    /// <summary>
    /// 状态码页面。
    /// 根据不同的 HTTP 状态码，展示不同的错误说明，帮助用户理解发生了什么。
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCodePage(int code)
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = code
        };

        (model.Title, model.Message) = code switch
        {
            404 => ("页面不存在", "你访问的页面可能已被移动、删除，或输入的地址不正确。"),
            403 => ("没有访问权限", "当前账号没有权限访问此页面，请联系管理员或切换账号后重试。"),
            401 => ("登录状态已失效", "当前登录状态可能已经过期，请重新登录后再继续操作。"),
            _ => ("请求暂时无法完成", "系统已返回错误状态，请稍后重试。")
        };

        return View("StatusCode", model);
    }
}
