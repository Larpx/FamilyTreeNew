using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IMenuService
{
    Task<List<MenuResponseDto>> GetAllAsync();
    
    Task<List<MenuResponseDto>> GetAllWithTreeAsync();
    
    Task<List<MenuResponseDto>> GetEnabledMenusAsync();
    
    Task<MenuResponseDto?> GetByIdAsync(Guid id);
    
    Task<MenuResponseDto> CreateAsync(MenuCreateRequestDto dto);
    
    Task<MenuResponseDto?> UpdateAsync(Guid id, MenuUpdateRequestDto dto);
    
    Task<bool> DeleteAsync(Guid id);
}