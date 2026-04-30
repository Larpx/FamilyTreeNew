using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 配偶关系创建请求DTO
/// </summary>
public class SpousalRelationCreateRequestDto
{
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    [Required(ErrorMessage = "丈夫ID不能为空")]
    public Guid HusbandId { get; set; }

    [Required(ErrorMessage = "妻子ID不能为空")]
    public Guid WifeId { get; set; }

    public DateTime? MarriageDateSolar { get; set; }

    [StringLength(50, ErrorMessage = "农历日期不能超过50个字符")]
    public string? MarriageDateLunar { get; set; }

    [StringLength(20, ErrorMessage = "状态不能超过20个字符")]
    public string? Status { get; set; }

    public bool IsDivorced { get; set; } = false;

    public DateTime? DivorceDateSolar { get; set; }

    [StringLength(50, ErrorMessage = "农历离婚日期不能超过50个字符")]
    public string? DivorceDateLunar { get; set; }

    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Remarks { get; set; }
}

/// <summary>
/// 配偶关系更新请求DTO
/// </summary>
public class SpousalRelationUpdateRequestDto
{
    public DateTime? MarriageDateSolar { get; set; }

    [StringLength(50, ErrorMessage = "农历日期不能超过50个字符")]
    public string? MarriageDateLunar { get; set; }

    [StringLength(20, ErrorMessage = "状态不能超过20个字符")]
    public string? Status { get; set; }

    public bool IsDivorced { get; set; }

    public DateTime? DivorceDateSolar { get; set; }

    [StringLength(50, ErrorMessage = "农历离婚日期不能超过50个字符")]
    public string? DivorceDateLunar { get; set; }

    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Remarks { get; set; }
}

/// <summary>
/// 配偶关系响应DTO
/// </summary>
public class SpousalRelationResponseDto
{
    public Guid Id { get; set; }

    public Guid FamilyTreeId { get; set; }

    public Guid HusbandId { get; set; }

    public string HusbandName { get; set; } = string.Empty;

    public Guid WifeId { get; set; }

    public string WifeName { get; set; } = string.Empty;

    public DateTime? MarriageDateSolar { get; set; }

    public string? MarriageDateLunar { get; set; }

    public string? Status { get; set; }

    public bool IsDivorced { get; set; }

    public DateTime? DivorceDateSolar { get; set; }

    public string? DivorceDateLunar { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
