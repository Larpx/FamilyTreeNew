using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 角色仓储接口。
/// 负责按编码查找角色、查询角色权限以及按管理员查询角色集合。
/// </summary>
public interface IRoleRepository : IBaseRepositoryGuid<Role>
{
    Task<Role?> GetByCodeAsync(string code);

    Task<Role?> GetWithPermissionsAsync(Guid id);

    Task<List<Role>> GetAllWithPermissionsAsync();

    Task<List<Role>> GetRolesByAdminIdAsync(Guid adminId);
}