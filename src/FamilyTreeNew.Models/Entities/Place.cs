using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

[SugarTable("Places")]
public class Place
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "地点ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "地点名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "详细地址")]
    public string? Address { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "行政区划-省")]
    public string? Province { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "行政区划-市")]
    public string? City { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "行政区划-县/区")]
    public string? District { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "纬度")]
    public decimal? Latitude { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "经度")]
    public decimal? Longitude { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "地点描述")]
    public string? Description { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }
}