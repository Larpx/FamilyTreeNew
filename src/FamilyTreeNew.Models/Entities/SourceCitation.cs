using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 来源引用实体。
/// 用来记录某条资料来源被引用到哪个成员、事件或家谱对象上。
/// </summary>
[SugarTable("SourceCitations")]
public class SourceCitation
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "引用ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false, ColumnDescription = "来源ID")]
    public Guid SourceId { get; set; }

    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "目标类型(Member/Event/FamilyTree)")]
    public string TargetType { get; set; } = string.Empty;

    [SugarColumn(IsNullable = false, ColumnDescription = "目标ID")]
    public Guid TargetId { get; set; }

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "引用说明")]
    public string? Note { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Navigate(NavigateType.OneToOne, nameof(SourceId))]
    public Source? Source { get; set; }
}