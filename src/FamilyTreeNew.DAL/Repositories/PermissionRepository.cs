using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IPermissionRepository : IBaseRepositoryGuid<Permission>
{
    Task<Permission?> GetByCodeAsync(string code);
    
    Task<List<Permission>> GetAllWithChildrenAsync();
    
    Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId);
}