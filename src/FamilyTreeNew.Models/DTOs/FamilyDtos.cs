using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建家族信息时使用的请求模型。
/// 用于在某个家谱下新增一个家族分支，并可同时填写地址和描述。
/// </summary>
public class FamilyCreateRequestDto
{
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    [Required(ErrorMessage = "家族名称不能为空")]
    [StringLength(100, ErrorMessage = "家族名称不能超过100个字符")]
    public string FamilyName { get; set; } = string.Empty;

    public Guid? HeadMemberId { get; set; }

    [StringLength(500, ErrorMessage = "家庭地址不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(2000, ErrorMessage = "家族描述不能超过2000个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 更新家族信息时使用的请求模型。
/// 该模型与创建模型类似，但用于修改已经存在的家族资料。
/// </summary>
public class FamilyUpdateRequestDto
{
    [Required(ErrorMessage = "家族名称不能为空")]
    [StringLength(100, ErrorMessage = "家族名称不能超过100个字符")]
    public string FamilyName { get; set; } = string.Empty;

    public Guid? HeadMemberId { get; set; }

    [StringLength(500, ErrorMessage = "家庭地址不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(2000, ErrorMessage = "家族描述不能超过2000个字符")]
    public string? Description { get; set; }
}

/// <summary>
/// 家族信息返回模型。
/// 前端拿到这个对象后，可以直接显示家族的名称、负责人和说明信息。
/// </summary>
public class FamilyResponseDto
{
    public int Id { get; set; }

    public Guid FamilyTreeId { get; set; }

    public string FamilyName { get; set; } = string.Empty;

    public Guid? HeadMemberId { get; set; }

    public string? HeadMemberName { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 创建配偶关系时使用的请求模型。
/// 主要用于录入一对夫妻的基础信息和婚姻状态。
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
/// 更新配偶关系时使用的请求模型。
/// 一般用于修改婚姻日期、离婚日期、状态和备注等内容。
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
/// 配偶关系返回模型。
/// 前端可以用它展示夫妻双方姓名、婚姻信息和关系状态。
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
