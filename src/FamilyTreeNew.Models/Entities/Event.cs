using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 事件实体
/// 记录家谱成员的各类事件，如出生、逝世、迁徙等
/// </summary>
[SugarTable("Events")]
public class Event
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "事件ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false, ColumnDescription = "事件类型ID")]
    public Guid EventTypeId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "家谱ID")]
    public Guid FamilyTreeId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "关联成员ID")]
    public Guid MemberId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "地点ID")]
    public Guid? PlaceId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "日期（公历）")]
    public DateTime? DateSolar { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "日期（农历）")]
    public string? DateLunar { get; set; }

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "事件描述")]
    public string? Description { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "是否主要事件")]
    public bool IsPrimary { get; set; } = false;

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "备注")]
    public string? Remarks { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(EventTypeId))]
    public EventType? EventType { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(PlaceId))]
    public Place? Place { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(MemberId))]
    public FamilyMember? Member { get; set; }
}
