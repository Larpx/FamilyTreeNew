using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<MenuResponseDto>> GetAllAsync()
    {
        var entities = await _menuRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<MenuResponseDto>> GetAllWithTreeAsync()
    {
        var entities = await _menuRepository.GetAllWithChildrenAsync();
        return entities.Select(MapToDtoWithChildren).ToList();
    }

    public async Task<List<MenuResponseDto>> GetEnabledMenusAsync()
    {
        var entities = await _menuRepository.GetEnabledMenusAsync();
        return entities.Select(MapToDtoWithChildren).ToList();
    }

    public async Task<MenuResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _menuRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<MenuResponseDto> CreateAsync(MenuCreateRequestDto dto)
    {
        var entity = new Menu
        {
            ParentId = dto.ParentId,
            Name = dto.Name,
            Url = dto.Url,
            Icon = dto.Icon,
            PermissionCode = dto.PermissionCode,
            SortOrder = dto.SortOrder,
            IsEnabled = dto.IsEnabled,
            IsVisible = dto.IsVisible,
            CreatedAt = DateTime.Now
        };

        await _menuRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    public async Task<MenuResponseDto?> UpdateAsync(Guid id, MenuUpdateRequestDto dto)
    {
        var entity = await _menuRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.ParentId = dto.ParentId;
        entity.Name = dto.Name;
        entity.Url = dto.Url;
        entity.Icon = dto.Icon;
        entity.PermissionCode = dto.PermissionCode;
        entity.SortOrder = dto.SortOrder;
        entity.IsEnabled = dto.IsEnabled;
        entity.IsVisible = dto.IsVisible;
        entity.UpdatedAt = DateTime.Now;

        await _menuRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _menuRepository.ExistsAsync(id)) return false;
        await _menuRepository.DeleteAsync(id);
        return true;
    }

    private static MenuResponseDto MapToDto(Menu entity)
    {
        return new MenuResponseDto
        {
            Id = entity.Id,
            ParentId = entity.ParentId,
            Name = entity.Name,
            Url = entity.Url,
            Icon = entity.Icon,
            PermissionCode = entity.PermissionCode,
            SortOrder = entity.SortOrder,
            IsEnabled = entity.IsEnabled,
            IsVisible = entity.IsVisible,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static MenuResponseDto MapToDtoWithChildren(Menu entity)
    {
        var dto = MapToDto(entity);
        dto.Children = entity.Children?.Select(MapToDtoWithChildren).ToList();
        return dto;
    }
}