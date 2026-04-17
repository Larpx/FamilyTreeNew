using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

[SugarTable("Permissions")]
public class Permission
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "权限ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "权限编码")]
    public string Code { get; set; } = string.Empty;

    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "权限名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "权限类型(menu/button/api)")]
    public string Type { get; set; } = string.Empty;

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "权限URL")]
    public string? Url { get; set; }

    [SugarColumn(Length = 10, IsNullable = true, ColumnDescription = "HTTP方法")]
    public string? Method { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "父权限ID")]
    public Guid? ParentId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "排序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Navigate(typeof(RolePermission), nameof(RolePermission.PermissionId), nameof(RolePermission.RoleId), new string[0])]
    public List<Role>? Roles { get; set; }

    /// <summary>
    /// 父权限
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public Permission? Parent { get; set; }

    /// <summary>
    /// 子权限列表
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(ParentId))]
    public List<Permission>? Children { get; set; }
}