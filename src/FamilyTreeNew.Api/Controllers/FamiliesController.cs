using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 家族控制器，提供家族的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FamiliesController : ControllerBase
{
    private readonly IFamilyService _familyService;
    private readonly ILogger<FamiliesController> _logger;

    public FamiliesController(IFamilyService familyService, ILogger<FamiliesController> logger)
    {
        _familyService = familyService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有家族列表
    /// </summary>
    /// <returns>家族列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FamilyResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<FamilyResponseDto>>>> GetAll()
    {
        try
        {
            var result = await _familyService.GetAllFamiliesAsync();
            return Ok(ApiResponse<List<FamilyResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家族列表失败");
            return StatusCode(500, ApiResponse<List<FamilyResponseDto>>.Fail("获取家族列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 根据ID获取家族详情
    /// </summary>
    /// <param name="id">家族ID</param>
    /// <returns>家族详情</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyResponseDto>>> GetById(int id)
    {
        try
        {
            var result = await _familyService.GetFamilyByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<FamilyResponseDto>.Fail("家族不存在"));
            }
            return Ok(ApiResponse<FamilyResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取家族详情失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<FamilyResponseDto>.Fail("获取家族详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建家族
    /// </summary>
    /// <param name="dto">家族创建数据</param>
    /// <returns>创建的家族信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<FamilyResponseDto>>> Create([FromBody] FamilyCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _familyService.CreateFamilyAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<FamilyResponseDto>.Ok(result, "家族创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建家族失败");
            return StatusCode(500, ApiResponse<FamilyResponseDto>.Fail("创建家族失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新家族信息
    /// </summary>
    /// <param name="id">家族ID</param>
    /// <param name="dto">家族更新数据</param>
    /// <returns>更新后的家族信息</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<FamilyResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FamilyResponseDto>>> Update(int id, [FromBody] FamilyUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<FamilyResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _familyService.UpdateFamilyAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<FamilyResponseDto>.Fail("家族不存在"));
            }
            return Ok(ApiResponse<FamilyResponseDto>.Ok(result, "家族更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新家族失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse<FamilyResponseDto>.Fail("更新家族失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除家族
    /// </summary>
    /// <param name="id">家族ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        try
        {
            var result = await _familyService.DeleteFamilyAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("家族不存在"));
            }
            return Ok(ApiResponse.Ok("家族删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除家族失败，ID: {Id}", id);
            return StatusCode(500, ApiResponse.Fail("删除家族失败，请稍后重试"));
        }
    }
}
