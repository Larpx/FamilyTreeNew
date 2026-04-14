using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

[SugarTable("Roles")]
public class Role
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "角色ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "角色名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "角色描述")]
    public string? Description { get; set; }

    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "角色编码")]
    public string Code { get; set; } = string.Empty;

    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }

    [Navigate(NavigateType.ManyToMany, nameof(RolePermission), nameof(RoleId), nameof(PermissionId))]
    public List<Permission>? Permissions { get; set; }

    [Navigate(NavigateType.ManyToMany, nameof(UserRole), nameof(RoleId), nameof(AdminId))]
    public List<Admin>? Admins { get; set; }
}