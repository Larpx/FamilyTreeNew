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
    /// 家族名称，如"张氏家族"
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string FamilyName { get; set; } = string.Empty;

    /// <summary>
    /// 家族简介描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 家族始祖成员ID，指向家族树的根节点
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? RootMemberId { get; set; }

    /// <summary>
    /// 家族记录创建时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 家族记录最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
