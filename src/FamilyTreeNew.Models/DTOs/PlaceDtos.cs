using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

public class PlaceCreateRequestDto
{
    [Required(ErrorMessage = "地点名称不能为空")]
    [StringLength(100, ErrorMessage = "地点名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "地址不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(50, ErrorMessage = "省份不能超过50个字符")]
    public string? Province { get; set; }

    [StringLength(50, ErrorMessage = "城市不能超过50个字符")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "区县不能超过50个字符")]
    public string? District { get; set; }

    [Range(-90, 90, ErrorMessage = "纬度范围必须在-90到90之间")]
    public decimal? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "经度范围必须在-180到180之间")]
    public decimal? Longitude { get; set; }

    [StringLength(2000, ErrorMessage = "描述不能超过2000个字符")]
    public string? Description { get; set; }

    public bool IsEnabled { get; set; } = true;
}

public class PlaceUpdateRequestDto
{
    [Required(ErrorMessage = "地点名称不能为空")]
    [StringLength(100, ErrorMessage = "地点名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "地址不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(50, ErrorMessage = "省份不能超过50个字符")]
    public string? Province { get; set; }

    [StringLength(50, ErrorMessage = "城市不能超过50个字符")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "区县不能超过50个字符")]
    public string? District { get; set; }

    [Range(-90, 90, ErrorMessage = "纬度范围必须在-90到90之间")]
    public decimal? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "经度范围必须在-180到180之间")]
    public decimal? Longitude { get; set; }

    [StringLength(2000, ErrorMessage = "描述不能超过2000个字符")]
    public string? Description { get; set; }

    public bool IsEnabled { get; set; }
}

public class PlaceResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Description { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
