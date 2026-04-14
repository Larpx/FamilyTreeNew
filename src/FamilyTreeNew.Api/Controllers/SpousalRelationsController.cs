using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpousalRelationsController : ControllerBase
{
    private readonly ISpousalRelationService _spousalRelationService;

    public SpousalRelationsController(ISpousalRelationService spousalRelationService)
    {
        _spousalRelationService = spousalRelationService;
    }

    [HttpGet("familytree/{familyTreeId}")]
    [ProducesResponseType(typeof(ApiResponse<List<SpousalRelationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SpousalRelationResponseDto>>>> GetByFamilyTreeId(Guid familyTreeId)
    {
        try
        {
            var result = await _spousalRelationService.GetByFamilyTreeIdAsync(familyTreeId);
            return Ok(ApiResponse<List<SpousalRelationResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SpousalRelationResponseDto>>.Fail($"获取配偶关系列表失败: {ex.Message}"));
        }
    }

    [HttpGet("member/{memberId}")]
    [ProducesResponseType(typeof(ApiResponse<List<SpousalRelationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SpousalRelationResponseDto>>>> GetByMemberId(Guid memberId)
    {
        try
        {
            var result = await _spousalRelationService.GetByMemberIdAsync(memberId);
            return Ok(ApiResponse<List<SpousalRelationResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SpousalRelationResponseDto>>.Fail($"获取成员配偶关系失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SpousalRelationResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _spousalRelationService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<SpousalRelationResponseDto>.Fail("配偶关系不存在"));
            }
            return Ok(ApiResponse<SpousalRelationResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SpousalRelationResponseDto>.Fail($"获取配偶关系详情失败: {ex.Message}"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SpousalRelationResponseDto>>> Create([FromBody] SpousalRelationCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<SpousalRelationResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _spousalRelationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SpousalRelationResponseDto>.Ok(result, "配偶关系创建成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SpousalRelationResponseDto>.Fail($"创建配偶关系失败: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SpousalRelationResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SpousalRelationResponseDto>>> Update(Guid id, [FromBody] SpousalRelationUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<SpousalRelationResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _spousalRelationService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<SpousalRelationResponseDto>.Fail("配偶关系不存在"));
            }
            return Ok(ApiResponse<SpousalRelationResponseDto>.Ok(result, "配偶关系更新成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SpousalRelationResponseDto>.Fail($"更新配偶关系失败: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _spousalRelationService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("配偶关系不存在"));
            }
            return Ok(ApiResponse.Ok("配偶关系删除成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"删除配偶关系失败: {ex.Message}"));
        }
    }
}