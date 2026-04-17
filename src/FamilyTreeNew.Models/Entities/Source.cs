using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 来源实体。
/// 用来保存家谱资料的来源信息，例如书籍、网站、档案或口述记录。
/// </summary>
[SugarTable("Sources")]
public class Source
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "来源ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 200, IsNullable = false, ColumnDescription = "来源标题")]
    public string Title { get; set; } = string.Empty;

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "作者")]
    public string? Author { get; set; }

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "出版社")]
    public string? Publisher { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "出版年份")]
    public int? Year { get; set; }

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "来源URL")]
    public string? Url { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "来源类型")]
    public string? Type { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "来源描述")]
    public string? Description { get; set; }

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "引用信息")]
    public string? Citation { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }
}