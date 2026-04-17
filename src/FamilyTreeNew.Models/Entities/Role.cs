using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 角色实体。
/// 用来表示用户可以拥有的身份，例如管理员、编辑者或普通访客。
/// </summary>
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

    [Navigate(typeof(RolePermission), nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId), new string[0])]
    public List<Permission>? Permissions { get; set; }

    [Navigate(typeof(UserRole), nameof(UserRole.RoleId), nameof(UserRole.AdminId), new string[0])]
    public List<Admin>? Admins { get; set; }
}