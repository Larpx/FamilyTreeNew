using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;

    public EventsController(IEventService eventService, IEventTypeService eventTypeService)
    {
        _eventService = eventService;
        _eventTypeService = eventTypeService;
    }

    [HttpGet("familytree/{familyTreeId}")]
    [ProducesResponseType(typeof(ApiResponse<List<EventResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventResponseDto>>>> GetByFamilyTreeId(Guid familyTreeId)
    {
        try
        {
            var result = await _eventService.GetByFamilyTreeIdAsync(familyTreeId);
            return Ok(ApiResponse<List<EventResponseDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<List<EventResponseDto>>.Fail("获取事件列表失败，请稍后重试"));
        }
    }

    [HttpGet("member/{memberId}")]
    [ProducesResponseType(typeof(ApiResponse<List<EventResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventResponseDto>>>> GetByMemberId(Guid memberId)
    {
        try
        {
            var result = await _eventService.GetByMemberIdAsync(memberId);
            return Ok(ApiResponse<List<EventResponseDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<List<EventResponseDto>>.Fail("获取成员事件失败，请稍后重试"));
        }
    }

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
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<EventResponseDto>.Fail("获取事件详情失败，请稍后重试"));
        }
    }

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
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<EventResponseDto>.Fail("创建事件失败，请稍后重试"));
        }
    }

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
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<EventResponseDto>.Fail("更新事件失败，请稍后重试"));
        }
    }

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
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.Fail("删除事件失败，请稍后重试"));
        }
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof(ApiResponse<List<EventTypeResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EventTypeResponseDto>>>> GetTypes()
    {
        try
        {
            var result = await _eventTypeService.GetEnabledTypesAsync();
            return Ok(ApiResponse<List<EventTypeResponseDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<List<EventTypeResponseDto>>.Fail("获取事件类型失败，请稍后重试"));
        }
    }
}