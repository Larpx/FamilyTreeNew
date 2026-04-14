using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建家谱请求DTO
/// </summary>
public class FamilyTreeCreateDto
{
    /// <summary>
    /// 家谱名称
    /// </summary>
    [Required(ErrorMessage = "家谱名称不能为空")]
    [StringLength(200, ErrorMessage = "家谱名称不能超过200个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 家谱简介（支持富文本）
    /// </summary>
    [StringLength(5000, ErrorMessage = "家谱简介不能超过5000个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 是否需要验证才能访问
    /// </summary>
    public bool RequireVerification { get; set; } = false;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新家谱请求DTO
/// </summary>
public class FamilyTreeUpdateDto
{
    /// <summary>
    /// 家谱名称
    /// </summary>
    [Required(ErrorMessage = "家谱名称不能为空")]
    [StringLength(200, ErrorMessage = "家谱名称不能超过200个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 家谱简介（支持富文本）
    /// </summary>
    [StringLength(5000, ErrorMessage = "家谱简介不能超过5000个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 是否需要验证才能访问
    /// </summary>
    public bool RequireVerification { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}

/// <summary>
/// 家谱响应DTO
/// </summary>
public class FamilyTreeDto
{
    /// <summary>
    /// 家谱ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 家谱名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 家谱简介
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 是否需要验证才能访问
    /// </summary>
    public bool RequireVerification { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 成员数量
    /// </summary>
    public int MemberCount { get; set; }
}

/// <summary>
/// 家谱查询请求DTO
/// </summary>
public class FamilyTreeQueryDto
{
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
    /// 关键词（搜索家谱名称）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 是否启用（可选，用于筛选）
    /// </summary>
    public bool? IsEnabled { get; set; }
}
