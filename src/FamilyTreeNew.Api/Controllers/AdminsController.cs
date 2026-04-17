using System.Security.Claims;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminsController : ControllerBase
{
    private readonly IAdminManagementService _adminManagementService;

    public AdminsController(IAdminManagementService adminManagementService)
    {
        _adminManagementService = adminManagementService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminDto>>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null)
    {
        var result = await _adminManagementService.GetPagedAsync(page, pageSize, keyword);
        return Ok(ApiResponse<PagedResult<AdminDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AdminDto>>> GetById(Guid id)
    {
        var result = await _adminManagementService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(ApiResponse<object>.Fail("管理员不存在", 404));
        }

        return Ok(ApiResponse<AdminDto>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AdminDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AdminDto>>> Create([FromBody] CreateAdminDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("数据验证失败", 400, GetValidationErrors()));
        }

        try
        {
            var result = await _adminManagementService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<AdminDto>.Ok(result, "管理员创建成功"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AdminDto>>> Update(Guid id, [FromBody] UpdateAdminDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("数据验证失败", 400, GetValidationErrors()));
        }

        try
        {
            var result = await _adminManagementService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<object>.Fail("管理员不存在", 404));
            }

            return Ok(ApiResponse<AdminDto>.Ok(result, "管理员更新成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(currentAdminId, out var loginAdminId) && loginAdminId == id)
        {
            return BadRequest(ApiResponse.Fail("不能删除当前登录的管理员账号"));
        }

        try
        {
            var result = await _adminManagementService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("管理员不存在", 404));
            }

            return Ok(ApiResponse.Ok("管理员删除成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    private List<string> GetValidationErrors()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToList();
    }
}
