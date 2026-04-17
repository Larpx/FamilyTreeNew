using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IRoleService
{
    Task<PagedResult<RoleResponseDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);
    
    Task<RoleResponseDto?> GetByIdAsync(Guid id);
    
    Task<RoleResponseDto?> GetByCodeAsync(string code);
    
    Task<RoleResponseDto> CreateAsync(RoleCreateRequestDto dto);
    
    Task<RoleResponseDto?> UpdateAsync(Guid id, RoleUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
    
    Task<bool> AssignPermissionsAsync(Guid roleId, List<Guid> permissionIds);
    
    Task<List<RoleResponseDto>> GetRolesByAdminIdAsync(Guid adminId);
}