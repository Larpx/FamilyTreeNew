using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 相册管理控制器
/// 提供相册的增删改查功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumService _albumService;
    private readonly IFamilyTreeService _familyTreeService;
    private readonly ILogger<AlbumsController> _logger;

    public AlbumsController(IAlbumService albumService, IFamilyTreeService familyTreeService, ILogger<AlbumsController> logger)
    {
        _albumService = albumService;
        _familyTreeService = familyTreeService;
        _logger = logger;
    }

    /// <summary>
    /// 获取相册列表
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AlbumDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AlbumDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResult<AlbumDto>>>> GetList([FromQuery] AlbumQueryDto query)
    {
        try
        {
            var result = await _albumService.GetAlbumsAsync(query);
            return Ok(ApiResponse<PagedResult<AlbumDto>>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<PagedResult<AlbumDto>>.Fail("获取相册列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 获取相册详情
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AlbumDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDetailDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AlbumDetailDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _albumService.GetAlbumByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<AlbumDetailDto>.Fail("相册不存在"));
            }
            return Ok(ApiResponse<AlbumDetailDto>.Ok(result));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<AlbumDetailDto>.Fail("获取相册详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 创建相册
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AlbumDto>>> Create([FromBody] AlbumCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<AlbumDto>.Fail("数据验证失败", 400, errors));
            }

            if (!await _familyTreeService.ExistsAsync(dto.FamilyTreeId))
            {
                return NotFound(ApiResponse<AlbumDto>.Fail("家谱不存在"));
            }

            var result = await _albumService.CreateAlbumAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<AlbumDto>.Ok(result, "相册创建成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<AlbumDto>.Fail("创建相册失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新相册
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<AlbumDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AlbumDto>>> Update(Guid id, [FromBody] AlbumUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<AlbumDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _albumService.UpdateAlbumAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<AlbumDto>.Fail("相册不存在"));
            }
            return Ok(ApiResponse<AlbumDto>.Ok(result, "相册更新成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<AlbumDto>.Fail("更新相册失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除相册
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _albumService.DeleteAlbumAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("相册不存在"));
            }
            return Ok(ApiResponse.Ok("相册删除成功"));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse.Fail("删除相册失败，请稍后重试"));
        }
    }
}
