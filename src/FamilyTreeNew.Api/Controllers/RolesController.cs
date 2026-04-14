using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RoleResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleResponseDto>>>> GetList(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        try
        {
            var result = await _roleService.GetPagedAsync(pageIndex, pageSize, keyword);
            return Ok(ApiResponse<PagedResult<RoleResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<RoleResponseDto>>.Fail($"获取角色列表失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoleResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _roleService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<RoleResponseDto>.Fail("角色不存在"));
            }
            return Ok(ApiResponse<RoleResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<RoleResponseDto>.Fail($"获取角色详情失败: {ex.Message}"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<RoleResponseDto>>> Create([FromBody] RoleCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<RoleResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RoleResponseDto>.Ok(result, "角色创建成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<RoleResponseDto>.Fail($"创建角色失败: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoleResponseDto>>> Update(Guid id, [FromBody] RoleUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<RoleResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _roleService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<RoleResponseDto>.Fail("角色不存在"));
            }
            return Ok(ApiResponse<RoleResponseDto>.Ok(result, "角色更新成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<RoleResponseDto>.Fail($"更新角色失败: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _roleService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("角色不存在"));
            }
            return Ok(ApiResponse.Ok("角色删除成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"删除角色失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PermissionResponseDto>>>> GetRolePermissions(Guid id)
    {
        try
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponse<List<PermissionResponseDto>>.Fail("角色不存在"));
            }

            var result = await _roleService.GetByIdAsync(id);
            return Ok(ApiResponse<List<PermissionResponseDto>>.Ok(result?.Permissions ?? new List<PermissionResponseDto>()));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PermissionResponseDto>>.Fail($"获取角色权限失败: {ex.Message}"));
        }
    }

    [HttpPut("{id}/permissions")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> AssignPermissions(Guid id, [FromBody] RolePermissionAssignRequestDto dto)
    {
        try
        {
            var result = await _roleService.AssignPermissionsAsync(id, dto.PermissionIds);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("角色不存在"));
            }
            return Ok(ApiResponse.Ok("权限分配成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"分配权限失败: {ex.Message}"));
        }
    }
}