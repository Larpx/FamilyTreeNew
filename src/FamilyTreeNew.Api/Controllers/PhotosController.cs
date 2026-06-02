using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

/// <summary>
/// 照片管理控制器
/// 提供照片的上传、查询、更新、删除和成员关联功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;
    private readonly IAlbumService _albumService;
    private readonly ILogger<PhotosController> _logger;

    public PhotosController(IPhotoService photoService, IAlbumService albumService, ILogger<PhotosController> logger)
    {
        _photoService = photoService;
        _albumService = albumService;
        _logger = logger;
    }

    /// <summary>
    /// 获取照片列表
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PhotoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PhotoDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedResult<PhotoDto>>>> GetList([FromQuery] PhotoQueryDto query)
    {
        try
        {
            var result = await _photoService.GetPhotosAsync(query);
            return Ok(ApiResponse<PagedResult<PhotoDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取照片列表失败");
            return StatusCode(500, ApiResponse<PagedResult<PhotoDto>>.Fail("获取照片列表失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 获取照片详情
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PhotoDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _photoService.GetPhotoByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<PhotoDto>.Fail("照片不存在"));
            }
            return Ok(ApiResponse<PhotoDto>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取照片详情失败，照片ID: {Id}", id);
            return StatusCode(500, ApiResponse<PhotoDto>.Fail("获取照片详情失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 上传照片
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResponse<List<PhotoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<PhotoDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<PhotoDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<PhotoDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<PhotoDto>>>> Upload([FromForm] PhotoUploadDto dto, List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(ApiResponse<List<PhotoDto>>.Fail("请选择要上传的照片"));
            }

            var validationErrors = new List<string>();
            foreach (var file in files)
            {
                var (isValid, errorMessage) = FileHelper.ValidateImageFile(file.FileName, file.Length);
                if (!isValid)
                {
                    validationErrors.Add($"{file.FileName}: {errorMessage}");
                    continue;
                }

                var (contentValid, contentError) = FileHelper.ValidateFileContent(file.FileName, file.OpenReadStream());
                if (!contentValid)
                {
                    validationErrors.Add($"{file.FileName}: {contentError}");
                }
            }

            if (validationErrors.Count > 0)
            {
                return BadRequest(ApiResponse<List<PhotoDto>>.Fail("文件验证失败", 400, validationErrors));
            }

            var albumDetail = await _albumService.GetAlbumByIdAsync(dto.AlbumId);
            if (albumDetail == null)
            {
                return NotFound(ApiResponse<List<PhotoDto>>.Fail("相册不存在"));
            }

            var uploadItems = files.Select(f => new PhotoUploadItem
            {
                Stream = f.OpenReadStream(),
                FileName = f.FileName,
                Length = f.Length
            }).ToList();

            var result = await _photoService.UploadPhotosAsync(dto.AlbumId, uploadItems, dto);
            return Ok(ApiResponse<List<PhotoDto>>.Ok(result, $"成功上传{result.Count}张照片"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "上传照片参数错误");
            return BadRequest(ApiResponse<List<PhotoDto>>.Fail("操作失败，请稍后重试"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上传照片失败");
            return StatusCode(500, ApiResponse<List<PhotoDto>>.Fail("上传照片失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 更新照片信息
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PhotoDto>>> Update(Guid id, [FromBody] PhotoUpdateDto dto)
    {
        try
        {
            var result = await _photoService.UpdatePhotoAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<PhotoDto>.Fail("照片不存在"));
            }
            return Ok(ApiResponse<PhotoDto>.Ok(result, "照片更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新照片失败，照片ID: {Id}", id);
            return StatusCode(500, ApiResponse<PhotoDto>.Fail("更新照片失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 删除照片
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _photoService.DeletePhotoAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("照片不存在"));
            }
            return Ok(ApiResponse.Ok("照片删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除照片失败，照片ID: {Id}", id);
            return StatusCode(500, ApiResponse.Fail("删除照片失败，请稍后重试"));
        }
    }

    /// <summary>
    /// 关联照片与成员
    /// </summary>
    [HttpPost("{id}/link-member")]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PhotoDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PhotoDto>>> LinkMember(Guid id, [FromBody] LinkMemberDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<PhotoDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _photoService.LinkMemberAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<PhotoDto>.Fail("照片不存在"));
            }
            return Ok(ApiResponse<PhotoDto>.Ok(result, dto.SetAsAvatar ? "照片已关联成员并设置为头像" : "照片已关联成员"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "关联照片成员参数错误，照片ID: {Id}", id);
            return BadRequest(ApiResponse<PhotoDto>.Fail("操作失败，请稍后重试"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "关联照片成员失败，照片ID: {Id}", id);
            return StatusCode(500, ApiResponse<PhotoDto>.Fail("关联成员失败，请稍后重试"));
        }
    }
}
