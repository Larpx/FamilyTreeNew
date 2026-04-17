using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerificationQuestionsController : ControllerBase
{
    private readonly IVerificationQuestionService _service;
    private readonly ILogger<VerificationQuestionsController> _logger;

    public VerificationQuestionsController(
        IVerificationQuestionService service,
        ILogger<VerificationQuestionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<VerificationQuestionDto>>>> GetAll()
    {
        try
        {
            var questions = await _service.GetAllAsync();
            return Ok(ApiResponse<List<VerificationQuestionDto>>.Ok(questions, "获取验证问题列表成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证问题列表失败");
            return StatusCode(500, ApiResponse<List<VerificationQuestionDto>>.Fail("获取验证问题列表失败"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<VerificationQuestionDto>>> GetById(Guid id)
    {
        try
        {
            var question = await _service.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound(ApiResponse<VerificationQuestionDto>.Fail($"验证问题ID {id} 不存在", 404));
            }
            return Ok(ApiResponse<VerificationQuestionDto>.Ok(question, "获取验证问题详情成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取验证问题详情失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<VerificationQuestionDto>.Fail("获取验证问题详情失败"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<VerificationQuestionDto>>> Create([FromBody] CreateVerificationQuestionDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VerificationQuestionDto>.Fail("请求参数验证失败", 400,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var question = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = question.Id },
                ApiResponse<VerificationQuestionDto>.Ok(question, "创建验证问题成功"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "创建验证问题失败：参数错误");
            return BadRequest(ApiResponse<VerificationQuestionDto>.Fail("操作失败，请稍后重试"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建验证问题失败");
            return StatusCode(500, ApiResponse<VerificationQuestionDto>.Fail("创建验证问题失败"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<VerificationQuestionDto>>> Update(Guid id, [FromBody] UpdateVerificationQuestionDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VerificationQuestionDto>.Fail("请求参数验证失败", 400,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var question = await _service.UpdateAsync(id, dto);
            if (question == null)
            {
                return NotFound(ApiResponse<VerificationQuestionDto>.Fail($"验证问题ID {id} 不存在", 404));
            }
            return Ok(ApiResponse<VerificationQuestionDto>.Ok(question, "更新验证问题成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新验证问题失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<VerificationQuestionDto>.Fail("更新验证问题失败"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.Fail($"验证问题ID {id} 不存在", 404));
            }
            return Ok(ApiResponse.Ok("删除验证问题成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除验证问题失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse.Fail("删除验证问题失败"));
        }
    }
}
