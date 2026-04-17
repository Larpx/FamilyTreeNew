using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 菜单实体。
/// 用来保存后台或前台菜单的层级结构、跳转地址、图标和显示状态。
/// </summary>
[SugarTable("Menus")]
public class Menu
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "菜单ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = true, ColumnDescription = "父菜单ID")]
    public Guid? ParentId { get; set; }

    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "菜单名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "菜单URL")]
    public string? Url { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "图标样式")]
    public string? Icon { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "权限编码")]
    public string? PermissionCode { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "排序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "是否显示")]
    public bool IsVisible { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public Menu? Parent { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public List<Menu>? Children { get; set; }
}