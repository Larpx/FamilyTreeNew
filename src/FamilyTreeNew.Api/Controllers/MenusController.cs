using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<MenuResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<MenuResponseDto>>>> GetList()
    {
        try
        {
            var result = await _menuService.GetAllAsync();
            return Ok(ApiResponse<List<MenuResponseDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<List<MenuResponseDto>>.Fail("获取菜单列表失败，请稍后重试"));
        }
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<MenuResponseDto>>>> GetTree()
    {
        try
        {
            var result = await _menuService.GetAllWithTreeAsync();
            return Ok(ApiResponse<List<MenuResponseDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<List<MenuResponseDto>>.Fail("获取菜单树失败，请稍后重试"));
        }
    }

    [HttpGet("enabled")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<MenuResponseDto>>>> GetEnabledMenus()
    {
        try
        {
            var result = await _menuService.GetEnabledMenusAsync();
            return Ok(ApiResponse<List<MenuResponseDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<List<MenuResponseDto>>.Fail("获取启用菜单失败，请稍后重试"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MenuResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _menuService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<MenuResponseDto>.Fail("菜单不存在"));
            }
            return Ok(ApiResponse<MenuResponseDto>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<MenuResponseDto>.Fail("获取菜单详情失败，请稍后重试"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MenuResponseDto>>> Create([FromBody] MenuCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<MenuResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _menuService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<MenuResponseDto>.Ok(result, "菜单创建成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<MenuResponseDto>.Fail("创建菜单失败，请稍后重试"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MenuResponseDto>>> Update(Guid id, [FromBody] MenuUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<MenuResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _menuService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<MenuResponseDto>.Fail("菜单不存在"));
            }
            return Ok(ApiResponse<MenuResponseDto>.Ok(result, "菜单更新成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<MenuResponseDto>.Fail("更新菜单失败，请稍后重试"));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _menuService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("菜单不存在"));
            }
            return Ok(ApiResponse.Ok("菜单删除成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.Fail("删除菜单失败，请稍后重试"));
        }
    }
}