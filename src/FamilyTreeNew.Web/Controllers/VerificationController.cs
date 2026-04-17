using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Web.Controllers;

public class VerificationController : Controller
{
    private readonly IVerificationService _verificationService;
    private readonly IFamilyTreeService _familyTreeService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<VerificationController> _logger;
    private readonly IConfiguration _configuration;

    public VerificationController(
        IVerificationService verificationService,
        IFamilyTreeService familyTreeService,
        IHttpClientFactory httpClientFactory,
        ILogger<VerificationController> logger,
        IConfiguration configuration)
    {
        _verificationService = verificationService;
        _familyTreeService = familyTreeService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Verify(Guid familyTreeId, int questionOrder = 1)
    {
        try
        {
            if (familyTreeId == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }

            var familyTree = await _familyTreeService.GetByIdAsync(familyTreeId);
            if (familyTree == null)
            {
                TempData["ErrorMessage"] = "家谱不存在";
                return RedirectToAction("Index", "Home");
            }

            if (!familyTree.RequireVerification)
            {
                return RedirectToAction("Detail", "FamilyTree", new { id = familyTreeId });
            }

            var status = await _verificationService.GetFamilyTreeVerificationStatusAsync(familyTreeId);

            if (status.TotalQuestions == 0)
            {
                TempData["ErrorMessage"] = "该家谱暂未设置验证问题";
                return RedirectToAction("Index", "Home");
            }

            var currentQuestion = status.Questions
                .OrderBy(q => q.Order)
                .FirstOrDefault(q => q.Order >= questionOrder);

            if (currentQuestion == null)
            {
                TempData["ErrorMessage"] = "验证问题不存在";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new VerificationViewModel
            {
                FamilyTreeId = familyTreeId,
                FamilyTreeName = familyTree.Name,
                FamilyTreeDescription = familyTree.Description,
                CurrentQuestion = currentQuestion,
                CurrentQuestionOrder = currentQuestion.Order,
                TotalQuestions = status.TotalQuestions,
                Progress = (int)((double)(currentQuestion.Order - 1) / status.TotalQuestions * 100)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证页面失败，家谱ID: {FamilyTreeId}", familyTreeId);
            TempData["ErrorMessage"] = "系统错误，请稍后重试";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(VerificationSubmitModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "请填写答案";
                return RedirectToAction(nameof(Verify), new { familyTreeId = model.FamilyTreeId, questionOrder = model.CurrentQuestionOrder });
            }

            var verifyDto = new VerifyAnswerDto
            {
                FamilyTreeId = model.FamilyTreeId,
                QuestionId = model.QuestionId,
                Answer = model.Answer?.Trim() ?? string.Empty
            };

            var result = await _verificationService.VerifyAnswerAsync(verifyDto);

            if (result.Success)
            {
                if (result.AllQuestionsPassed)
                {
                    if (!string.IsNullOrEmpty(result.AccessToken))
                    {
                        Response.Cookies.Append($"ft_access_{model.FamilyTreeId}", result.AccessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddHours(24)
                        });
                    }

                    TempData["SuccessMessage"] = "验证成功！欢迎访问家谱";
                    return RedirectToAction("Detail", "FamilyTree", new { id = model.FamilyTreeId });
                }
                else
                {
                    int nextOrder = result.NextQuestionOrder ?? model.CurrentQuestionOrder + 1;
                    TempData["SuccessMessage"] = "答案正确，请继续回答下一题";
                    return RedirectToAction(nameof(Verify), new { familyTreeId = model.FamilyTreeId, questionOrder = nextOrder });
                }
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "答案不正确，请重试";
                return RedirectToAction(nameof(Verify), new { familyTreeId = model.FamilyTreeId, questionOrder = model.CurrentQuestionOrder });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证答案失败，家谱ID: {FamilyTreeId}", model.FamilyTreeId);
            TempData["ErrorMessage"] = "系统错误，请稍后重试";
            return RedirectToAction(nameof(Verify), new { familyTreeId = model.FamilyTreeId, questionOrder = model.CurrentQuestionOrder });
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckAccess(Guid familyTreeId)
    {
        try
        {
            if (familyTreeId == Guid.Empty)
            {
                return Json(new { hasAccess = false });
            }

            var familyTree = await _familyTreeService.GetByIdAsync(familyTreeId);
            if (familyTree == null)
            {
                return Json(new { hasAccess = false, message = "家谱不存在" });
            }

            if (!familyTree.RequireVerification)
            {
                return Json(new { hasAccess = true, redirectUrl = Url.Action("Detail", "FamilyTree", new { id = familyTreeId }) });
            }

            string? token = Request.Cookies[$"ft_access_{familyTreeId}"];
            if (!string.IsNullOrEmpty(token))
            {
                bool isValid = _verificationService.ValidateAccessToken(token, familyTreeId);
                if (isValid)
                {
                    return Json(new { hasAccess = true, redirectUrl = Url.Action("Detail", "FamilyTree", new { id = familyTreeId }) });
                }
            }

            return Json(new { hasAccess = false, redirectUrl = Url.Action(nameof(Verify), new { familyTreeId }) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查访问权限失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return Json(new { hasAccess = false, message = "系统错误" });
        }
    }

    [HttpPost]
    public IActionResult ClearToken(Guid familyTreeId)
    {
        Response.Cookies.Delete($"ft_access_{familyTreeId}");
        return Ok();
    }
}

public class VerificationViewModel
{
    public Guid FamilyTreeId { get; set; }
    public string FamilyTreeName { get; set; } = string.Empty;
    public string? FamilyTreeDescription { get; set; }
    public VerificationQuestionDto CurrentQuestion { get; set; } = new();
    public int CurrentQuestionOrder { get; set; }
    public int TotalQuestions { get; set; }
    public int Progress { get; set; }
}

public class VerificationSubmitModel
{
    public Guid FamilyTreeId { get; set; }
    public Guid QuestionId { get; set; }
    public int CurrentQuestionOrder { get; set; }
    public string? Answer { get; set; }
}
