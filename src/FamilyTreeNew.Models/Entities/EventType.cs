using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

[SugarTable("EventTypes")]
public class EventType
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "事件类型ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "事件类型名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "事件类型编码")]
    public string Code { get; set; } = string.Empty;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "事件类型描述")]
    public string? Description { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "排序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Navigate(NavigateType.OneToMany, nameof(Event.EventTypeId))]
    public List<Event>? Events { get; set; }
}