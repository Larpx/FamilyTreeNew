using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 验证问题响应DTO
/// </summary>
public class VerificationQuestionDto
{
    /// <summary>
    /// 问题ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 所属家谱ID
    /// </summary>
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 问题内容
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 验证顺序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 家谱名称
    /// </summary>
    public string? FamilyTreeName { get; set; }
}

/// <summary>
/// 创建验证问题请求DTO
/// </summary>
public class CreateVerificationQuestionDto
{
    /// <summary>
    /// 所属家谱ID
    /// </summary>
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 问题内容
    /// </summary>
    [Required(ErrorMessage = "问题内容不能为空")]
    [StringLength(500, ErrorMessage = "问题内容长度不能超过500个字符")]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 答案关键词（用于验证用户回答）
    /// </summary>
    [Required(ErrorMessage = "答案关键词不能为空")]
    [StringLength(200, ErrorMessage = "答案关键词长度不能超过200个字符")]
    public string AnswerKeyword { get; set; } = string.Empty;

    /// <summary>
    /// 验证顺序
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "验证顺序必须大于0")]
    public int Order { get; set; } = 1;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新验证问题请求DTO
/// </summary>
public class UpdateVerificationQuestionDto
{
    /// <summary>
    /// 问题内容
    /// </summary>
    [Required(ErrorMessage = "问题内容不能为空")]
    [StringLength(500, ErrorMessage = "问题内容长度不能超过500个字符")]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// 答案关键词
    /// </summary>
    [Required(ErrorMessage = "答案关键词不能为空")]
    [StringLength(200, ErrorMessage = "答案关键词长度不能超过200个字符")]
    public string AnswerKeyword { get; set; } = string.Empty;

    /// <summary>
    /// 验证顺序
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "验证顺序必须大于0")]
    public int Order { get; set; } = 1;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}
