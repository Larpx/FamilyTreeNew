using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 权限仓储接口。
/// 负责查询权限树、按编码查找权限，以及按角色获取权限集合。
/// </summary>
public interface IPermissionRepository : IBaseRepositoryGuid<Permission>
{
    Task<Permission?> GetByCodeAsync(string code);

    Task<List<Permission>> GetAllWithChildrenAsync();

    Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId);
}