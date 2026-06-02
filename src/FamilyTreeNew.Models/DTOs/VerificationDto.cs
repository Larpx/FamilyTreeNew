using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 验证回答请求DTO
/// </summary>
public class VerifyAnswerDto
{
    /// <summary>
    /// 家谱ID
    /// </summary>
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 问题ID
    /// </summary>
    [Required(ErrorMessage = "问题ID不能为空")]
    public Guid QuestionId { get; set; }

    /// <summary>
    /// 用户回答
    /// </summary>
    [Required(ErrorMessage = "答案不能为空")]
    [StringLength(500, ErrorMessage = "答案长度不能超过500个字符")]
    public string Answer { get; set; } = string.Empty;
}

/// <summary>
/// 验证结果响应DTO
/// </summary>
public class VerificationResultDto
{
    /// <summary>
    /// 是否验证成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 结果消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 访问令牌（验证成功时返回）
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 下一个问题序号（多问题验证时）
    /// </summary>
    public int? NextQuestionOrder { get; set; }

    /// <summary>
    /// 是否所有问题都已通过
    /// </summary>
    public bool AllQuestionsPassed { get; set; }
}

/// <summary>
/// 家谱验证状态响应DTO
/// </summary>
public class FamilyTreeVerificationStatusDto
{
    /// <summary>
    /// 家谱ID
    /// </summary>
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 家谱名称
    /// </summary>
    public string FamilyTreeName { get; set; } = string.Empty;

    /// <summary>
    /// 是否需要验证
    /// </summary>
    public bool RequireVerification { get; set; }

    /// <summary>
    /// 验证问题总数
    /// </summary>
    public int TotalQuestions { get; set; }

    /// <summary>
    /// 验证问题列表
    /// </summary>
    public List<VerificationQuestionDto> Questions { get; set; } = new();
}

/// <summary>
/// 令牌验证请求DTO
/// </summary>
public class ValidateTokenRequestDto
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    [Required(ErrorMessage = "令牌不能为空")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 家谱ID
    /// </summary>
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }
}
