using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 事件类型控制器，提供事件类型的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventTypesController : ControllerBase
{
    private readonly IEventTypeService _eventTypeService;
    private readonly ILogger<EventTypesController> _logger;

    public EventTypesController(IEventTypeService eventTypeService, ILogger<EventTypesController> logger)
    {
        _eventTypeService = eventTypeService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有事件类型列表
    /// </summary>
    /// <returns>事件类型列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<EventTypeResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventTypeResponseDto>>>> GetList()
    {
        try
        {
            var result = await _eventTypeService.GetAllAsync();
            return Ok(ApiResponse<List<EventTypeResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取事件类型列表失败");
            return StatusCode(500, ApiResponse<List<EventTypeResponseDto>>.Fail("获取事件类型列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 获取启用的事件类型列表
    /// </summary>
    /// <returns>启用的事件类型列表</returns>
    [HttpGet("enabled")]
    [ProducesResponseType(typeof(ApiResponse<List<EventTypeResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventTypeResponseDto>>>> GetEnabledTypes()
    {
        try
        {
            var result = await _eventTypeService.GetEnabledTypesAsync();
            return Ok(ApiResponse<List<EventTypeResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用的事件类型失败");
            return StatusCode(500, ApiResponse<List<EventTypeResponseDto>>.Fail("获取启用的事件类型失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据ID获取事件类型详情
    /// </summary>
    /// <param name="id">事件类型ID</param>
    /// <returns>事件类型详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventTypeResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _eventTypeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<EventTypeResponseDto>.Fail("事件类型不存在"));
            }
            return Ok(ApiResponse<EventTypeResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取事件类型 {Id} 详情失败", id);
            return StatusCode(500, ApiResponse<EventTypeResponseDto>.Fail("获取事件类型详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建事件类型
    /// </summary>
    /// <param name="dto">事件类型创建数据</param>
    /// <returns>创建的事件类型信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EventTypeResponseDto>>> Create([FromBody] EventTypeCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<EventTypeResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _eventTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EventTypeResponseDto>.Ok(result, "事件类型创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建事件类型失败");
            return StatusCode(500, ApiResponse<EventTypeResponseDto>.Fail("创建事件类型失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新事件类型
    /// </summary>
    /// <param name="id">事件类型ID</param>
    /// <param name="dto">事件类型更新数据</param>
    /// <returns>更新后的事件类型信息</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EventTypeResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventTypeResponseDto>>> Update(Guid id, [FromBody] EventTypeUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<EventTypeResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _eventTypeService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<EventTypeResponseDto>.Fail("事件类型不存在"));
            }
            return Ok(ApiResponse<EventTypeResponseDto>.Ok(result, "事件类型更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新事件类型 {Id} 失败", id);
            return StatusCode(500, ApiResponse<EventTypeResponseDto>.Fail("更新事件类型失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除事件类型
    /// </summary>
    /// <param name="id">事件类型ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _eventTypeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("事件类型不存在"));
            }
            return Ok(ApiResponse.Ok("事件类型删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除事件类型 {Id} 失败", id);
            return StatusCode(500, ApiResponse.Fail("删除事件类型失败，请稍后重试"));
        }
    }
}
