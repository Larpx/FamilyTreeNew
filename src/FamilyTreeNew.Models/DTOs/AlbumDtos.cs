using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建相册请求DTO
/// </summary>
public class AlbumCreateDto
{
    /// <summary>
    /// 所属家谱ID
    /// </summary>
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 相册名称
    /// </summary>
    [Required(ErrorMessage = "相册名称不能为空")]
    [StringLength(200, ErrorMessage = "相册名称不能超过200个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 相册描述
    /// </summary>
    [StringLength(500, ErrorMessage = "相册描述不能超过500个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 更新相册请求DTO
/// </summary>
public class AlbumUpdateDto
{
    /// <summary>
    /// 相册名称
    /// </summary>
    [Required(ErrorMessage = "相册名称不能为空")]
    [StringLength(200, ErrorMessage = "相册名称不能超过200个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 相册描述
    /// </summary>
    [StringLength(500, ErrorMessage = "相册描述不能超过500个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 相册响应DTO
/// </summary>
public class AlbumDto
{
    /// <summary>
    /// 相册ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 所属家谱ID
    /// </summary>
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 相册名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 相册描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 照片数量
    /// </summary>
    public int PhotoCount { get; set; }

    /// <summary>
    /// 封面照片路径
    /// </summary>
    public string? CoverPhotoPath { get; set; }
}

/// <summary>
/// 相册详情响应DTO（包含照片列表）
/// </summary>
public class AlbumDetailDto : AlbumDto
{
    /// <summary>
    /// 相册中的照片列表
    /// </summary>
    public List<PhotoDto> Photos { get; set; } = new();
}

/// <summary>
/// 相册查询请求DTO
/// </summary>
public class AlbumQueryDto
{
    /// <summary>
    /// 家谱ID（可选，用于筛选特定家谱的相册）
    /// </summary>
    public Guid? FamilyTreeId { get; set; }

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    [Range(1, 100, ErrorMessage = "每页大小必须在1-100之间")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 关键词（搜索相册名称）
    /// </summary>
    public string? Keyword { get; set; }
}
