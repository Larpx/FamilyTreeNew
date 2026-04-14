using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PermissionResponseDto>>>> GetList()
    {
        try
        {
            var result = await _permissionService.GetAllAsync();
            return Ok(ApiResponse<List<PermissionResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PermissionResponseDto>>.Fail($"获取权限列表失败: {ex.Message}"));
        }
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PermissionResponseDto>>>> GetTree()
    {
        try
        {
            var result = await _permissionService.GetAllWithTreeAsync();
            return Ok(ApiResponse<List<PermissionResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PermissionResponseDto>>.Fail($"获取权限树失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PermissionResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _permissionService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<PermissionResponseDto>.Fail("权限不存在"));
            }
            return Ok(ApiResponse<PermissionResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PermissionResponseDto>.Fail($"获取权限详情失败: {ex.Message}"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PermissionResponseDto>>> Create([FromBody] PermissionCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<PermissionResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _permissionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<PermissionResponseDto>.Ok(result, "权限创建成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PermissionResponseDto>.Fail($"创建权限失败: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PermissionResponseDto>>> Update(Guid id, [FromBody] PermissionUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<PermissionResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _permissionService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<PermissionResponseDto>.Fail("权限不存在"));
            }
            return Ok(ApiResponse<PermissionResponseDto>.Ok(result, "权限更新成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PermissionResponseDto>.Fail($"更新权限失败: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _permissionService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("权限不存在"));
            }
            return Ok(ApiResponse.Ok("权限删除成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"删除权限失败: {ex.Message}"));
        }
    }
}