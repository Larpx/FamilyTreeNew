using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 家族实体类
/// 存储家族基本信息，一个家族可以包含多个家谱
/// </summary>
[SugarTable("Families")]
public class Family
{
    /// <summary>
    /// 家族唯一标识符，自增主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 家谱ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "家谱ID")]
    public Guid? FamilyTreeId { get; set; }

    /// <summary>
    /// 家族名称，如"张氏家族"
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "家族名称")]
    public string FamilyName { get; set; } = string.Empty;

    /// <summary>
    /// 户主成员ID，指向家族的户主
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "户主成员ID")]
    public Guid? HeadMemberId { get; set; }

    /// <summary>
    /// 家庭地址
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "家庭地址")]
    public string? Address { get; set; }

    /// <summary>
    /// 家族简介描述
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "家族描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 家族记录创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 家族记录最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 所属家谱
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(FamilyTreeId))]
    public FamilyTree? FamilyTree { get; set; }

    /// <summary>
    /// 户主成员
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(HeadMemberId))]
    public FamilyMember? HeadMember { get; set; }
}
