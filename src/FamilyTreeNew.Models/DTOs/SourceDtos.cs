using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建来源时使用的请求模型。
/// 用于录入资料来源的标题、作者、出版信息和引用说明。
/// </summary>
public class SourceCreateRequestDto
{
    [Required(ErrorMessage = "来源标题不能为空")]
    [StringLength(200, ErrorMessage = "来源标题不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "作者不能超过100个字符")]
    public string? Author { get; set; }

    [StringLength(100, ErrorMessage = "出版者不能超过100个字符")]
    public string? Publisher { get; set; }

    [Range(1, 9999, ErrorMessage = "年份必须在1-9999之间")]
    public int? Year { get; set; }

    [StringLength(500, ErrorMessage = "URL不能超过500个字符")]
    [Url(ErrorMessage = "URL格式不正确")]
    public string? Url { get; set; }

    [StringLength(20, ErrorMessage = "类型不能超过20个字符")]
    public string? Type { get; set; }

    [StringLength(2000, ErrorMessage = "描述不能超过2000个字符")]
    public string? Description { get; set; }

    [StringLength(2000, ErrorMessage = "引用信息不能超过2000个字符")]
    public string? Citation { get; set; }

    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新来源时使用的请求模型。
/// 与创建模型类似，但用于编辑已经存在的来源资料。
/// </summary>
public class SourceUpdateRequestDto
{
    [Required(ErrorMessage = "来源标题不能为空")]
    [StringLength(200, ErrorMessage = "来源标题不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "作者不能超过100个字符")]
    public string? Author { get; set; }

    [StringLength(100, ErrorMessage = "出版者不能超过100个字符")]
    public string? Publisher { get; set; }

    [Range(1, 9999, ErrorMessage = "年份必须在1-9999之间")]
    public int? Year { get; set; }

    [StringLength(500, ErrorMessage = "URL不能超过500个字符")]
    [Url(ErrorMessage = "URL格式不正确")]
    public string? Url { get; set; }

    [StringLength(20, ErrorMessage = "类型不能超过20个字符")]
    public string? Type { get; set; }

    [StringLength(2000, ErrorMessage = "描述不能超过2000个字符")]
    public string? Description { get; set; }

    [StringLength(2000, ErrorMessage = "引用信息不能超过2000个字符")]
    public string? Citation { get; set; }

    public bool IsEnabled { get; set; }
}

/// <summary>
/// 来源返回模型。
/// 前端可以通过它展示来源的完整详情，并用于引用关联。
/// </summary>
public class SourceResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Author { get; set; }

    public string? Publisher { get; set; }

    public int? Year { get; set; }

    public string? Url { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? Citation { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 创建来源引用时使用的请求模型。
/// 它把某个来源与具体成员、事件或家谱对象关联起来。
/// </summary>
public class SourceCitationCreateRequestDto
{
    [Required(ErrorMessage = "来源ID不能为空")]
    public Guid SourceId { get; set; }

    [Required(ErrorMessage = "目标类型不能为空")]
    [StringLength(20, ErrorMessage = "目标类型不能超过20个字符")]
    public string TargetType { get; set; } = string.Empty;

    [Required(ErrorMessage = "目标ID不能为空")]
    public Guid TargetId { get; set; }

    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Note { get; set; }
}

/// <summary>
/// 来源引用返回模型。
/// 方便前端查看某条资料被引用到了什么对象上。
/// </summary>
public class SourceCitationResponseDto
{
    public Guid Id { get; set; }

    public Guid SourceId { get; set; }

    public string SourceTitle { get; set; } = string.Empty;

    public string TargetType { get; set; } = string.Empty;

    public Guid TargetId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
}
