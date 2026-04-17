using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;
    private readonly IVerificationQuestionService _questionService;
    private readonly ILogger<VerificationController> _logger;

    public VerificationController(
        IVerificationService verificationService,
        IVerificationQuestionService questionService,
        ILogger<VerificationController> logger)
    {
        _verificationService = verificationService;
        _questionService = questionService;
        _logger = logger;
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponse<VerificationResultDto>>> VerifyAnswer([FromBody] VerifyAnswerDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VerificationResultDto>.Fail("请求参数验证失败", 400,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var result = await _verificationService.VerifyAnswerAsync(dto);
            return Ok(ApiResponse<VerificationResultDto>.Ok(result, "验证完成"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证答案失败，家谱ID: {FamilyTreeId}, 问题ID: {QuestionId}",
                dto.FamilyTreeId, dto.QuestionId);
            return StatusCode(500, ApiResponse<VerificationResultDto>.Fail("验证答案失败"));
        }
    }

    [HttpGet("familytrees/{familyTreeId}/status")]
    public async Task<ActionResult<ApiResponse<FamilyTreeVerificationStatusDto>>> GetFamilyTreeVerificationStatus(Guid familyTreeId)
    {
        try
        {
            var status = await _verificationService.GetFamilyTreeVerificationStatusAsync(familyTreeId);
            return Ok(ApiResponse<FamilyTreeVerificationStatusDto>.Ok(status, "获取验证状态成功"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "获取家谱验证状态失败：家谱不存在");
            return NotFound(ApiResponse<FamilyTreeVerificationStatusDto>.Fail("操作失败，请稍后重试", 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱验证状态失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return StatusCode(500, ApiResponse<FamilyTreeVerificationStatusDto>.Fail("获取家谱验证状态失败"));
        }
    }

    [HttpGet("familytrees/{familyTreeId}/questions")]
    public async Task<ActionResult<ApiResponse<List<VerificationQuestionDto>>>> GetFamilyTreeQuestions(Guid familyTreeId)
    {
        try
        {
            var questions = await _questionService.GetByFamilyTreeIdAsync(familyTreeId);
            return Ok(ApiResponse<List<VerificationQuestionDto>>.Ok(questions, "获取验证问题成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱验证问题失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return StatusCode(500, ApiResponse<List<VerificationQuestionDto>>.Fail("获取验证问题失败"));
        }
    }

    [HttpPost("familytrees/{familyTreeId}/questions")]
    public async Task<ActionResult<ApiResponse>> AddQuestionsToFamilyTree(
        Guid familyTreeId,
        [FromBody] List<CreateVerificationQuestionDto> questions)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail("请求参数验证失败", 400,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            if (questions == null || questions.Count == 0)
            {
                return BadRequest(ApiResponse.Fail("验证问题列表不能为空"));
            }

            foreach (var question in questions)
            {
                question.FamilyTreeId = familyTreeId;
            }

            var success = await _questionService.AddQuestionsToFamilyTreeAsync(familyTreeId, questions);
            if (!success)
            {
                return NotFound(ApiResponse.Fail($"家谱ID {familyTreeId} 不存在", 404));
            }

            return Ok(ApiResponse.Ok("验证问题添加成功"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "添加验证问题失败：参数错误");
            return BadRequest(ApiResponse.Fail("操作失败，请稍后重试"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加验证问题失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return StatusCode(500, ApiResponse.Fail("添加验证问题失败"));
        }
    }

    [HttpPost("validate-token")]
    public ActionResult<ApiResponse<bool>> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(ApiResponse<bool>.Fail("令牌不能为空"));
            }

            var isValid = _verificationService.ValidateAccessToken(request.Token, request.FamilyTreeId);
            return Ok(ApiResponse<bool>.Ok(isValid, isValid ? "令牌有效" : "令牌无效或已过期"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证令牌失败");
            return StatusCode(500, ApiResponse<bool>.Fail("验证令牌失败"));
        }
    }
}

public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public Guid FamilyTreeId { get; set; }
}
