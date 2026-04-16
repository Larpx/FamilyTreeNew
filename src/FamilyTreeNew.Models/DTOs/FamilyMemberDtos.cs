using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建家族成员请求DTO
/// </summary>
public class FamilyMemberCreateDto
{
    /// <summary>
    /// 所属家谱ID
    /// </summary>
    [Required(ErrorMessage = "家谱ID不能为空")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 父成员ID（可选，为null表示始祖）
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 姓氏
    /// </summary>
    [Required(ErrorMessage = "姓氏不能为空")]
    [StringLength(50, ErrorMessage = "姓氏不能超过50个字符")]
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// 名字
    /// </summary>
    [Required(ErrorMessage = "名字不能为空")]
    [StringLength(50, ErrorMessage = "名字不能超过50个字符")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 字号别称
    /// </summary>
    [StringLength(100, ErrorMessage = "字号别称不能超过100个字符")]
    public string? Alias { get; set; }

    /// <summary>
    /// 排行（如长子、次子等）
    /// </summary>
    [StringLength(20, ErrorMessage = "排行不能超过20个字符")]
    public string? Ranking { get; set; }

    /// <summary>
    /// 字辈
    /// </summary>
    [StringLength(50, ErrorMessage = "字辈不能超过50个字符")]
    public string? GenerationName { get; set; }

    /// <summary>
    /// 性别（M-男，F-女）
    /// </summary>
    [StringLength(10, ErrorMessage = "性别不能超过10个字符")]
    public string? Gender { get; set; }

    /// <summary>
    /// 出生日期（公历）
    /// </summary>
    public DateTime? BirthDateSolar { get; set; }

    /// <summary>
    /// 出生日期（农历）
    /// </summary>
    [StringLength(50, ErrorMessage = "生辰农历不能超过50个字符")]
    public string? BirthDateLunar { get; set; }

    /// <summary>
    /// 居住地
    /// </summary>
    [StringLength(200, ErrorMessage = "居住地不能超过200个字符")]
    public string? Residence { get; set; }

    /// <summary>
    /// 职业
    /// </summary>
    [StringLength(100, ErrorMessage = "职业不能超过100个字符")]
    public string? Occupation { get; set; }

    /// <summary>
    /// 个人详细信息
    /// </summary>
    [StringLength(2000, ErrorMessage = "个人信息不能超过2000个字符")]
    public string? PersonalInfo { get; set; }

    /// <summary>
    /// 小注（简短备注）
    /// </summary>
    [StringLength(500, ErrorMessage = "小注不能超过500个字符")]
    public string? Note { get; set; }

    /// <summary>
    /// 是否已逝世
    /// </summary>
    public bool IsDeceased { get; set; } = false;

    /// <summary>
    /// 逝世日期（农历）
    /// </summary>
    [StringLength(50, ErrorMessage = "卒亡农历不能超过50个字符")]
    public string? DeathDateLunar { get; set; }

    /// <summary>
    /// 逝世日期（公历）
    /// </summary>
    public DateTime? DeathDateSolar { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Remarks { get; set; }
}

/// <summary>
/// 更新家族成员请求DTO
/// </summary>
public class FamilyMemberUpdateDto
{
    /// <summary>
    /// 父成员ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 姓氏
    /// </summary>
    [Required(ErrorMessage = "姓氏不能为空")]
    [StringLength(50, ErrorMessage = "姓氏不能超过50个字符")]
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// 名字
    /// </summary>
    [Required(ErrorMessage = "名字不能为空")]
    [StringLength(50, ErrorMessage = "名字不能超过50个字符")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 字号别称
    /// </summary>
    [StringLength(100, ErrorMessage = "字号别称不能超过100个字符")]
    public string? Alias { get; set; }

    /// <summary>
    /// 排行
    /// </summary>
    [StringLength(20, ErrorMessage = "排行不能超过20个字符")]
    public string? Ranking { get; set; }

    /// <summary>
    /// 字辈
    /// </summary>
    [StringLength(50, ErrorMessage = "字辈不能超过50个字符")]
    public string? GenerationName { get; set; }

    /// <summary>
    /// 性别（M-男，F-女）
    /// </summary>
    [StringLength(10, ErrorMessage = "性别不能超过10个字符")]
    public string? Gender { get; set; }

    /// <summary>
    /// 出生日期（公历）
    /// </summary>
    public DateTime? BirthDateSolar { get; set; }

    /// <summary>
    /// 出生日期（农历）
    /// </summary>
    [StringLength(50, ErrorMessage = "生辰农历不能超过50个字符")]
    public string? BirthDateLunar { get; set; }

    /// <summary>
    /// 居住地
    /// </summary>
    [StringLength(200, ErrorMessage = "居住地不能超过200个字符")]
    public string? Residence { get; set; }

    /// <summary>
    /// 职业
    /// </summary>
    [StringLength(100, ErrorMessage = "职业不能超过100个字符")]
    public string? Occupation { get; set; }

    /// <summary>
    /// 个人详细信息
    /// </summary>
    [StringLength(2000, ErrorMessage = "个人信息不能超过2000个字符")]
    public string? PersonalInfo { get; set; }

    /// <summary>
    /// 小注
    /// </summary>
    [StringLength(500, ErrorMessage = "小注不能超过500个字符")]
    public string? Note { get; set; }

    /// <summary>
    /// 是否已逝世
    /// </summary>
    public bool IsDeceased { get; set; }

    /// <summary>
    /// 逝世日期（农历）
    /// </summary>
    [StringLength(50, ErrorMessage = "卒亡农历不能超过50个字符")]
    public string? DeathDateLunar { get; set; }

    /// <summary>
    /// 逝世日期（公历）
    /// </summary>
    public DateTime? DeathDateSolar { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(2000, ErrorMessage = "备注不能超过2000个字符")]
    public string? Remarks { get; set; }
}

/// <summary>
/// 家族成员响应DTO
/// </summary>
public class FamilyMemberDto
{
    /// <summary>
    /// 成员ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 所属家谱ID
    /// </summary>
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 父成员ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 世代数
    /// </summary>
    public int? Generation { get; set; }

    /// <summary>
    /// 姓氏
    /// </summary>
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// 名字
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 全名（姓+名）
    /// </summary>
    public string FullName => $"{Surname}{FirstName}";

    /// <summary>
    /// 字号别称
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// 排行
    /// </summary>
    public string? Ranking { get; set; }

    /// <summary>
    /// 字辈
    /// </summary>
    public string? GenerationName { get; set; }

    /// <summary>
    /// 性别（M-男，F-女）
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 出生日期（公历）
    /// </summary>
    public DateTime? BirthDateSolar { get; set; }

    /// <summary>
    /// 出生日期（农历）
    /// </summary>
    public string? BirthDateLunar { get; set; }

    /// <summary>
    /// 居住地
    /// </summary>
    public string? Residence { get; set; }

    /// <summary>
    /// 职业
    /// </summary>
    public string? Occupation { get; set; }

    /// <summary>
    /// 个人详细信息
    /// </summary>
    public string? PersonalInfo { get; set; }

    /// <summary>
    /// 小注
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// 是否已逝世
    /// </summary>
    public bool IsDeceased { get; set; }

    /// <summary>
    /// 逝世日期（农历）
    /// </summary>
    public string? DeathDateLunar { get; set; }

    /// <summary>
    /// 逝世日期（公历）
    /// </summary>
    public DateTime? DeathDateSolar { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 父成员姓名
    /// </summary>
    public string? ParentName { get; set; }
}

/// <summary>
/// 家族成员查询请求DTO
/// </summary>
public class FamilyMemberQueryDto
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
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 家谱ID（可选，用于筛选特定家谱的成员）
    /// </summary>
    public Guid? FamilyTreeId { get; set; }

    /// <summary>
    /// 关键词（搜索姓名、字号等）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 世代数（可选，用于筛选特定世代的成员）
    /// </summary>
    public int? Generation { get; set; }

    /// <summary>
    /// 父成员ID（可选，用于筛选特定父成员的子成员）
    /// </summary>
    public Guid? ParentId { get; set; }
}
