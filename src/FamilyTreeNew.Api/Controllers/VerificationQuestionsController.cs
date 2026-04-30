using Microsoft.AspNetCore.Mvc;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 验证问题控制器，提供验证问题的增删改查功能
/// </summary>
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

    /// <summary>
    /// 获取所有验证问题列表
    /// </summary>
    /// <returns>验证问题列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<VerificationQuestionDto>>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// 根据ID获取验证问题详情
    /// </summary>
    /// <param name="id">验证问题ID</param>
    /// <returns>验证问题详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// 创建验证问题
    /// </summary>
    /// <param name="dto">验证问题创建数据</param>
    /// <returns>创建的验证问题信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// 更新验证问题
    /// </summary>
    /// <param name="id">验证问题ID</param>
    /// <param name="dto">验证问题更新数据</param>
    /// <returns>更新后的验证问题信息</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<VerificationQuestionDto>), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// 删除验证问题
    /// </summary>
    /// <param name="id">验证问题ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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
