using FamilyTreeNew.BLL.Services;
using FamilyTreeNew.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeNew.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlacesController : ControllerBase
{
    private readonly IPlaceService _placeService;

    public PlacesController(IPlaceService placeService)
    {
        _placeService = placeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PlaceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PlaceResponseDto>>>> GetList()
    {
        try
        {
            var result = await _placeService.GetAllAsync();
            return Ok(ApiResponse<List<PlaceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PlaceResponseDto>>.Fail($"获取地点列表失败: {ex.Message}"));
        }
    }

    [HttpGet("enabled")]
    [ProducesResponseType(typeof(ApiResponse<List<PlaceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PlaceResponseDto>>>> GetEnabledPlaces()
    {
        try
        {
            var result = await _placeService.GetEnabledPlacesAsync();
            return Ok(ApiResponse<List<PlaceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PlaceResponseDto>>.Fail($"获取启用的地点失败: {ex.Message}"));
        }
    }

    [HttpGet("province/{province}")]
    [ProducesResponseType(typeof(ApiResponse<List<PlaceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PlaceResponseDto>>>> GetByProvince(string province)
    {
        try
        {
            var result = await _placeService.GetByProvinceAsync(province);
            return Ok(ApiResponse<List<PlaceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PlaceResponseDto>>.Fail($"获取省份地点失败: {ex.Message}"));
        }
    }

    [HttpGet("city/{city}")]
    [ProducesResponseType(typeof(ApiResponse<List<PlaceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PlaceResponseDto>>>> GetByCity(string city)
    {
        try
        {
            var result = await _placeService.GetByCityAsync(city);
            return Ok(ApiResponse<List<PlaceResponseDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<PlaceResponseDto>>.Fail($"获取城市地点失败: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PlaceResponseDto>>> GetById(Guid id)
    {
        try
        {
            var result = await _placeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<PlaceResponseDto>.Fail("地点不存在"));
            }
            return Ok(ApiResponse<PlaceResponseDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PlaceResponseDto>.Fail($"获取地点详情失败: {ex.Message}"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PlaceResponseDto>>> Create([FromBody] PlaceCreateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<PlaceResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _placeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<PlaceResponseDto>.Ok(result, "地点创建成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PlaceResponseDto>.Fail($"创建地点失败: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PlaceResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PlaceResponseDto>>> Update(Guid id, [FromBody] PlaceUpdateRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<PlaceResponseDto>.Fail("数据验证失败", 400, errors));
            }

            var result = await _placeService.UpdateAsync(id, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<PlaceResponseDto>.Fail("地点不存在"));
            }
            return Ok(ApiResponse<PlaceResponseDto>.Ok(result, "地点更新成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PlaceResponseDto>.Fail($"更新地点失败: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            var result = await _placeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse.Fail("地点不存在"));
            }
            return Ok(ApiResponse.Ok("地点删除成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail($"删除地点失败: {ex.Message}"));
        }
    }
}