using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

[SugarTable("SpousalRelations")]
public class SpousalRelation
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "配偶关系ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false, ColumnDescription = "家谱ID")]
    public Guid FamilyTreeId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "丈夫ID")]
    public Guid HusbandId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "妻子ID")]
    public Guid WifeId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "结婚日期（公历）")]
    public DateTime? MarriageDateSolar { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "结婚日期（农历）")]
    public string? MarriageDateLunar { get; set; }

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "婚姻状况说明")]
    public string? Status { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "是否离异")]
    public bool IsDivorced { get; set; } = false;

    [SugarColumn(IsNullable = true, ColumnDescription = "离婚日期（公历）")]
    public DateTime? DivorceDateSolar { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "离婚日期（农历）")]
    public string? DivorceDateLunar { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "备注")]
    public string? Remarks { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(HusbandId))]
    public FamilyMember? Husband { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(WifeId))]
    public FamilyMember? Wife { get; set; }
}