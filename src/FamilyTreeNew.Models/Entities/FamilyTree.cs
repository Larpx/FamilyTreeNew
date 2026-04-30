using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 家谱实体类
/// 存储家谱基本信息，一个家谱包含多个家族成员
/// </summary>
[SugarTable("FamilyTrees")]
public class FamilyTree
{
    /// <summary>
    /// 家谱唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "家谱ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 家谱名称，如"张氏家谱"
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "家谱名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 家谱简介，支持富文本格式
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "家谱简介（富文本）")]
    public string? Description { get; set; }

    /// <summary>
    /// 家谱创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建日期")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否需要验证才能查看，true表示需要回答验证问题
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否需要验证")]
    public bool RequireVerification { get; set; } = false;

    /// <summary>
    /// 家谱状态，true为启用，false为禁用
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "状态（启用/禁用）")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 家谱最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新日期")]
    public DateTime? UpdatedAt { get; set; }
}
