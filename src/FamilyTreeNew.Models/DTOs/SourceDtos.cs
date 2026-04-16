using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

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
