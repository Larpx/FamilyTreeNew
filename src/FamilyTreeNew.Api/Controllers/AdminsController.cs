using System.Security.Claims;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 管理员控制器，提供管理员的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminsController : ControllerBase
{
    private readonly IAdminManagementService _adminManagementService;
    private readonly ILogger<AdminsController> _logger;

    public AdminsController(IAdminManagementService adminManagementService, ILogger<AdminsController> logger)
    {
        _adminManagementService = adminManagementService;
        _logger = logger;
    }

    /// <summary>
    /// 获取管理员分页列表
    /// </summary>
    /// <param name="page">页码，默认1</param>
    /// <param name="pageSize">每页数量，默认10</param>
    /// <param name="keyword">搜索关键词</param>
    /// <returns>管理员分页列表</returns>
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

    /// <summary>
    /// 根据ID获取管理员详情
    /// </summary>
    /// <param name="id">管理员ID</param>
    /// <returns>管理员详情</returns>
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

    /// <summary>
    /// 创建管理员
    /// </summary>
    /// <param name="dto">管理员创建数据</param>
    /// <returns>创建的管理员信息</returns>
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
            _logger.LogWarning(ex, "创建管理员参数无效");
            return BadRequest(ApiResponse<object>.Fail("请求参数无效"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "创建管理员操作失败");
            return BadRequest(ApiResponse<object>.Fail("操作失败，请检查输入数据"));
        }
    }

    /// <summary>
    /// 更新管理员信息
    /// </summary>
    /// <param name="id">管理员ID</param>
    /// <param name="dto">管理员更新数据</param>
    /// <returns>更新后的管理员信息</returns>
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
            _logger.LogWarning(ex, "更新管理员 {Id} 操作失败", id);
            return BadRequest(ApiResponse<object>.Fail("操作失败，请检查输入数据"));
        }
    }

    /// <summary>
    /// 删除管理员
    /// </summary>
    /// <param name="id">管理员ID</param>
    /// <returns>删除结果</returns>
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
            _logger.LogWarning(ex, "删除管理员 {Id} 操作失败", id);
            return BadRequest(ApiResponse.Fail("操作失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 获取模型验证错误列表
    /// </summary>
    /// <returns>验证错误消息列表</returns>
    private List<string> GetValidationErrors()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToList();
    }
}
