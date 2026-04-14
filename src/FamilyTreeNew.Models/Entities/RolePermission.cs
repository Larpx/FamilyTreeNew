using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

[SugarTable("RolePermissions")]
public class RolePermission
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false, ColumnDescription = "角色ID")]
    public Guid RoleId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "权限ID")]
    public Guid PermissionId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Navigate(NavigateType.OneToOne, nameof(RoleId))]
    public Role? Role { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(PermissionId))]
    public Permission? Permission { get; set; }
}