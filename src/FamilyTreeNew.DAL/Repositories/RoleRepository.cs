using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IRoleRepository : IBaseRepositoryGuid<Role>
{
    Task<Role?> GetByCodeAsync(string code);
    
    Task<Role?> GetWithPermissionsAsync(Guid id);
    
    Task<List<Role>> GetAllWithPermissionsAsync();
    
    Task<List<Role>> GetRolesByAdminIdAsync(Guid adminId);
}