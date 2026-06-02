using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 事件控制器，提供事件的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    /// <summary>
    /// 根据家谱ID获取事件列表
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>事件列表</returns>
    [HttpGet("familytree/{familyTreeId}")]
    [ProducesResponseType(typeof(ApiResponse<List<EventResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventResponseDto>>>> GetByFamilyTreeId(Guid familyTreeId)
    {
        try
        {
            var result = await _eventService.GetByFamilyTreeIdAsync(familyTreeId);
            return Ok(ApiResponse<List<EventResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家谱 {FamilyTreeId} 的事件列表失败", familyTreeId);
            return StatusCode(500, ApiResponse<List<EventResponseDto>>.Fail("获取事件列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据成员ID获取事件列表
    /// </summary>
    /// <param name="memberId">成员ID</param>
    /// <returns>事件列表</returns>
    [HttpGet("member/{memberId}")]
    [ProducesResponseType(typeof(ApiResponse<List<EventResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventResponseDto>>>> GetByMemberId(Guid memberId)
    {
        try
        {
            var result = await _eventService.GetByMemberIdAsync(memberId);
            return Ok(ApiResponse<List<EventResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成员 {MemberId} 的事件列表失败", memberId);
            return StatusCode(500, ApiResponse<List<EventResponseDto>>.Fail("获取成员事件失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据ID获取事件详情
    /// </summary>
    /// <param name="id">事件ID</param>
    /// <returns>事件详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _eventService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<EventResponseDto>.Fail("事件不存在"));
            }
            return Ok(ApiResponse<EventResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取事件 {Id} 详情失败", id);
            return StatusCode(500, ApiResponse<EventResponseDto>.Fail("获取事件详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建事件
    /// </summary>
    /// <param name="dto">事件创建数据</param>
    /// <returns>创建的事件信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EventResponseDto>>> Create([FromBody] EventCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<EventResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EventResponseDto>.Ok(result, "事件创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建事件失败");
            return StatusCode(500, ApiResponse<EventResponseDto>.Fail("创建事件失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新事件
    /// </summary>
    /// <param name="id">事件ID</param>
    /// <param name="dto">事件更新数据</param>
    /// <returns>更新后的事件信息</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EventResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventResponseDto>>> Update(Guid id, [FromBody] EventUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<EventResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _eventService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<EventResponseDto>.Fail("事件不存在"));
            }
            return Ok(ApiResponse<EventResponseDto>.Ok(result, "事件更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新事件 {Id} 失败", id);
            return StatusCode(500, ApiResponse<EventResponseDto>.Fail("更新事件失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="id">事件ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _eventService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("事件不存在"));
            }
            return Ok(ApiResponse.Ok("事件删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除事件 {Id} 失败", id);
            return StatusCode(500, ApiResponse.Fail("删除事件失败，请稍后重试"));
        }
    }

}
