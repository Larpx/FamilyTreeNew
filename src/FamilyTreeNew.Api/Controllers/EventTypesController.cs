using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventTypesController : ControllerBase
{
    private readonly IEventTypeService _eventTypeService;

    public EventTypesController(IEventTypeService eventTypeService)
    {
        _eventTypeService = eventTypeService;
    }

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
            return StatusCode(500, ApiResponse<List<EventTypeResponseDto>>.Fail($"获取事件类型列表失败: {ex.Message}"));
        }
    }

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
            return StatusCode(500, ApiResponse<List<EventTypeResponseDto>>.Fail($"获取启用的事件类型失败: {ex.Message}"));
        }
    }

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
            return StatusCode(500, ApiResponse<EventTypeResponseDto>.Fail($"获取事件类型详情失败: {ex.Message}"));
        }
    }

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
            return StatusCode(500, ApiResponse<EventTypeResponseDto>.Fail($"创建事件类型失败: {ex.Message}"));
        }
    }

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
            return StatusCode(500, ApiResponse<EventTypeResponseDto>.Fail($"更新事件类型失败: {ex.Message}"));
        }
    }

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
            return StatusCode(500, ApiResponse.Fail($"删除事件类型失败: {ex.Message}"));
        }
    }
}