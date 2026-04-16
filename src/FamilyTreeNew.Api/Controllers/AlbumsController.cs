using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumService _albumService;
    private readonly IFamilyTreeService _familyTreeService;

    public AlbumsController(IAlbumService albumService, IFamilyTreeService familyTreeService)
    {
        _albumService = albumService;
        _familyTreeService = familyTreeService;
    }

    [HttpGet]
    [AllowAnonymous]
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

    [HttpGet("{id}")]
    [AllowAnonymous]
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

    [HttpPost]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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
