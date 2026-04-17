using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 用户角色关联实体。
/// 这是管理员和角色之间的中间表，用来表示一个管理员可以拥有多个角色。
/// </summary>
[SugarTable("UserRoles")]
public class UserRole
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(IsNullable = false, ColumnDescription = "管理员ID")]
    public Guid AdminId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "角色ID")]
    public Guid RoleId { get; set; }

    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Navigate(NavigateType.OneToOne, nameof(AdminId))]
    public Admin? Admin { get; set; }

    [Navigate(NavigateType.OneToOne, nameof(RoleId))]
    public Role? Role { get; set; }
}