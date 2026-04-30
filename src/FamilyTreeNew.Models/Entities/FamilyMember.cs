using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 家族成员实体类
/// 存储家谱中每个成员的详细信息，包括基本信息、生卒日期、家族关系等
/// </summary>
[SugarTable("FamilyMembers")]
public class FamilyMember
{
    /// <summary>
    /// 成员唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "成员ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 所属家谱ID，关联FamilyTree表
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "家谱ID")]
    public Guid FamilyTreeId { get; set; }

    /// <summary>
    /// 父成员ID，用于构建家族树结构，null表示始祖
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "父成员ID")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 世代数，从始祖开始计算，始祖为第1代
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "世代（自动计算）")]
    public int? Generation { get; set; }

    /// <summary>
    /// 姓氏
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "姓氏")]
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// 名字
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "名字")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 字号别称，如字、号等
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "字号别称")]
    public string? Alias { get; set; }

    /// <summary>
    /// 排行，如"长子"、"次子"等
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "排行")]
    public string? Ranking { get; set; }

    /// <summary>
    /// 字辈，用于标识同辈成员
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "字辈")]
    public string? GenerationName { get; set; }

    /// <summary>
    /// 性别（M-男，F-女）
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = true, ColumnDescription = "性别")]
    public string? Gender { get; set; }

    /// <summary>
    /// 出生日期（公历）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "生辰公历")]
    public DateTime? BirthDateSolar { get; set; }

    /// <summary>
    /// 出生日期（农历），格式如"甲子年正月初一"
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "生辰农历")]
    public string? BirthDateLunar { get; set; }

    /// <summary>
    /// 居住地地址
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "居住地")]
    public string? Residence { get; set; }

    /// <summary>
    /// 职业信息
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "职业")]
    public string? Occupation { get; set; }

    /// <summary>
    /// 个人详细信息，支持长文本
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "个人信息")]
    public string? PersonalInfo { get; set; }

    /// <summary>
    /// 小注，用于记录简短备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "小注")]
    public string? Note { get; set; }

    /// <summary>
    /// 是否已故，true表示已故
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "卒亡标识")]
    public bool IsDeceased { get; set; } = false;

    /// <summary>
    /// 逝世日期（农历）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "卒亡农历")]
    public string? DeathDateLunar { get; set; }

    /// <summary>
    /// 逝世日期（公历）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "卒亡公历")]
    public DateTime? DeathDateSolar { get; set; }

    /// <summary>
    /// 备注信息，支持长文本
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "备注")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 记录创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建日期")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新日期")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 导航属性：所属家谱
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(FamilyTreeId))]
    public FamilyTree? FamilyTree { get; set; }

    /// <summary>
    /// 导航属性：父成员
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public FamilyMember? Parent { get; set; }
}
