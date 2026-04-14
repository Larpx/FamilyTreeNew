using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IPermissionService
{
    Task<List<PermissionResponseDto>> GetAllAsync();
    
    Task<List<PermissionResponseDto>> GetAllWithTreeAsync();
    
    Task<PermissionResponseDto?> GetByIdAsync(Guid id);
    
    Task<PermissionResponseDto?> GetByCodeAsync(string code);
    
    Task<PermissionResponseDto> CreateAsync(PermissionCreateRequestDto dto);
    
    Task<PermissionResponseDto?> UpdateAsync(Guid id, PermissionUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
    
    Task<List<PermissionResponseDto>> GetPermissionsByRoleIdAsync(Guid roleId);
}