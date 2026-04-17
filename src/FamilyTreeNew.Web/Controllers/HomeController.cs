using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyTreeNew.Web.Models;
using FamilyTreeNew.BLL.Services;

namespace FamilyTreeNew.Web.Controllers;

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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
