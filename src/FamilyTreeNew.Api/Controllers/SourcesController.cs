using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SourcesController : ControllerBase
{
    private readonly ISourceService _sourceService;
    private readonly ISourceCitationService _sourceCitationService;

    public SourcesController(ISourceService sourceService, ISourceCitationService sourceCitationService)
    {
        _sourceService = sourceService;
        _sourceCitationService = sourceCitationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<SourceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SourceResponseDto>>>> GetList()
    {
        try
        {
            var result = await _sourceService.GetAllAsync();
            return Ok(ApiResponse<List<SourceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SourceResponseDto>>.Fail($"获取来源列表失败: {ex.Message}"));
        }
    }

    [HttpGet("enabled")]
    [ProducesResponseType(typeof(ApiResponse<List<SourceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SourceResponseDto>>>> GetEnabledSources()
    {
        try
        {
            var result = await _sourceService.GetEnabledSourcesAsync();
            return Ok(ApiResponse<List<SourceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SourceResponseDto>>.Fail($"获取启用的来源失败: {ex.Message}"));
        }
    }

    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(ApiResponse<List<SourceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SourceResponseDto>>>> GetByType(string type)
    {
        try
        {
            var result = await _sourceService.GetByTypeAsync(type);
            return Ok(ApiResponse<List<SourceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SourceResponseDto>>.Fail($"获取类型来源失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SourceResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _sourceService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<SourceResponseDto>.Fail("来源不存在"));
            }
            return Ok(ApiResponse<SourceResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SourceResponseDto>.Fail($"获取来源详情失败: {ex.Message}"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SourceResponseDto>>> Create([FromBody] SourceCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<SourceResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _sourceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SourceResponseDto>.Ok(result, "来源创建成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SourceResponseDto>.Fail($"创建来源失败: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SourceResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SourceResponseDto>>> Update(Guid id, [FromBody] SourceUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<SourceResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _sourceService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<SourceResponseDto>.Fail("来源不存在"));
            }
            return Ok(ApiResponse<SourceResponseDto>.Ok(result, "来源更新成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SourceResponseDto>.Fail($"更新来源失败: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _sourceService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("来源不存在"));
            }
            return Ok(ApiResponse.Ok("来源删除成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"删除来源失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}/citations")]
    [ProducesResponseType(typeof(ApiResponse<List<SourceCitationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SourceCitationResponseDto>>>> GetCitations(Guid id)
    {
        try
        {
            var result = await _sourceCitationService.GetBySourceIdAsync(id);
            return Ok(ApiResponse<List<SourceCitationResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SourceCitationResponseDto>>.Fail($"获取来源引用失败: {ex.Message}"));
        }
    }

    [HttpPost("citations")]
    [ProducesResponseType(typeof(ApiResponse<SourceCitationResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<SourceCitationResponseDto>>> CreateCitation([FromBody] SourceCitationCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<SourceCitationResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _sourceCitationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCitations), new { id = result.SourceId }, ApiResponse<SourceCitationResponseDto>.Ok(result, "来源引用创建成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SourceCitationResponseDto>.Fail($"创建来源引用失败: {ex.Message}"));
        }
    }

    [HttpDelete("citations/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteCitation(Guid id)
    {
        try
        {
            var result = await _sourceCitationService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("来源引用不存在"));
            }
            return Ok(ApiResponse.Ok("来源引用删除成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"删除来源引用失败: {ex.Message}"));
        }
    }
}