using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 角色仓储实现。
/// 提供角色按编码查询、角色权限加载以及按管理员查询角色的能力。
/// </summary>
public class RoleRepository : BaseRepositoryGuid<Role>, IRoleRepository
{
    public RoleRepository(SqlSugarContext context) : base(context) { }

    public async Task<Role?> GetByCodeAsync(string code)
    {
        return await Db.Queryable<Role>().Where(r => r.Code == code).FirstAsync();
    }

    public async Task<Role?> GetWithPermissionsAsync(Guid id)
    {
        return await Db.Queryable<Role>()
            .Includes(r => r.Permissions)
            .Where(r => r.Id == id)
            .FirstAsync();
    }

    public async Task<List<Role>> GetAllWithPermissionsAsync()
    {
        return await Db.Queryable<Role>()
            .Includes(r => r.Permissions)
            .ToListAsync();
    }

    public async Task<List<Role>> GetRolesByAdminIdAsync(Guid adminId)
    {
        return await Db.Queryable<Role>()
            .InnerJoin<UserRole>((r, ur) => r.Id == ur.RoleId)
            .Where((r, ur) => ur.AdminId == adminId)
            .Select((r, ur) => r)
            .ToListAsync();
    }
}