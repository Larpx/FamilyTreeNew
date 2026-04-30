using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 配偶关系控制器，提供配偶关系的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpousalRelationsController : ControllerBase
{
    private readonly ISpousalRelationService _spousalRelationService;
    private readonly ILogger<SpousalRelationsController> _logger;

    public SpousalRelationsController(ISpousalRelationService spousalRelationService, ILogger<SpousalRelationsController> logger)
    {
        _spousalRelationService = spousalRelationService;
        _logger = logger;
    }

    /// <summary>
    /// 根据家谱ID获取配偶关系列表
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>配偶关系列表</returns>
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
            _logger.LogError(ex, "获取家谱 {FamilyTreeId} 的配偶关系列表失败", familyTreeId);
            return StatusCode(500, ApiResponse<List<SpousalRelationResponseDto>>.Fail("获取配偶关系列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据成员ID获取配偶关系列表
    /// </summary>
    /// <param name="memberId">成员ID</param>
    /// <returns>配偶关系列表</returns>
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
            _logger.LogError(ex, "获取成员 {MemberId} 的配偶关系失败", memberId);
            return StatusCode(500, ApiResponse<List<SpousalRelationResponseDto>>.Fail("获取成员配偶关系失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据ID获取配偶关系详情
    /// </summary>
    /// <param name="id">配偶关系ID</param>
    /// <returns>配偶关系详情</returns>
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
            _logger.LogError(ex, "获取配偶关系 {Id} 详情失败", id);
            return StatusCode(500, ApiResponse<SpousalRelationResponseDto>.Fail("获取配偶关系详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建配偶关系
    /// </summary>
    /// <param name="dto">配偶关系创建数据</param>
    /// <returns>创建的配偶关系信息</returns>
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
            _logger.LogError(ex, "创建配偶关系失败");
            return StatusCode(500, ApiResponse<SpousalRelationResponseDto>.Fail("创建配偶关系失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新配偶关系
    /// </summary>
    /// <param name="id">配偶关系ID</param>
    /// <param name="dto">配偶关系更新数据</param>
    /// <returns>更新后的配偶关系信息</returns>
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
            _logger.LogError(ex, "更新配偶关系 {Id} 失败", id);
            return StatusCode(500, ApiResponse<SpousalRelationResponseDto>.Fail("更新配偶关系失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除配偶关系
    /// </summary>
    /// <param name="id">配偶关系ID</param>
    /// <returns>删除结果</returns>
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
            _logger.LogError(ex, "删除配偶关系 {Id} 失败", id);
            return StatusCode(500, ApiResponse.Fail("删除配偶关系失败，请稍后重试"));
        }
    }
}
