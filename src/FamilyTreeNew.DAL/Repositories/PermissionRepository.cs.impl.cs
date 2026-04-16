using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

public class PermissionRepository : BaseRepositoryGuid<Permission>, IPermissionRepository
{
    public PermissionRepository(SqlSugarContext context) : base(context) { }

    public async Task<Permission?> GetByCodeAsync(string code)
    {
        return await Db.Queryable<Permission>().Where(p => p.Code == code).FirstAsync();
    }

    public async Task<List<Permission>> GetAllWithChildrenAsync()
    {
        var permissions = await Db.Queryable<Permission>()
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        var rootPermissions = permissions.Where(p => p.ParentId == null).ToList();
        BuildPermissionTree(rootPermissions, permissions);

        return rootPermissions;
    }

    private void BuildPermissionTree(List<Permission> parents, List<Permission> allPermissions)
    {
        foreach (var parent in parents)
        {
            parent.Children = allPermissions
                .Where(p => p.ParentId == parent.Id)
                .ToList();
            
            if (parent.Children?.Count > 0)
            {
                BuildPermissionTree(parent.Children, allPermissions);
            }
        }
    }

    public async Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        return await Db.Queryable<Permission>()
            .InnerJoin<RolePermission>((p, rp) => p.Id == rp.PermissionId)
            .Where((p, rp) => rp.RoleId == roleId)
            .Select((p, rp) => p)
            .ToListAsync();
    }
}