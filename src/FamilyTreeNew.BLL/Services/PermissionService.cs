using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<List<PermissionResponseDto>> GetAllAsync()
    {
        var entities = await _permissionRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<PermissionResponseDto>> GetAllWithTreeAsync()
    {
        var entities = await _permissionRepository.GetAllWithChildrenAsync();
        return entities.Select(MapToDtoWithChildren).ToList();
    }

    public async Task<PermissionResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _permissionRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<PermissionResponseDto?> GetByCodeAsync(string code)
    {
        var entity = await _permissionRepository.GetByCodeAsync(code);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<PermissionResponseDto> CreateAsync(PermissionCreateRequestDto dto)
    {
        var existingPermission = await _permissionRepository.GetByCodeAsync(dto.Code);
        if (existingPermission != null)
        {
            throw new InvalidOperationException($"权限编码 '{dto.Code}' 已存在");
        }

        var entity = new Permission
        {
            Code = dto.Code,
            Name = dto.Name,
            Type = dto.Type,
            Url = dto.Url,
            Method = dto.Method,
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _permissionRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    public async Task<PermissionResponseDto?> UpdateAsync(Guid id, PermissionUpdateRequestDto dto)
    {
        var entity = await _permissionRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Type = dto.Type;
        entity.Url = dto.Url;
        entity.Method = dto.Method;
        entity.ParentId = dto.ParentId;
        entity.SortOrder = dto.SortOrder;
        entity.IsEnabled = dto.IsEnabled;

        await _permissionRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _permissionRepository.ExistsAsync(id)) return false;

        var allPermissions = await _permissionRepository.GetAllAsync();
        if (allPermissions.Any(p => p.ParentId == id))
        {
            throw new InvalidOperationException("该权限下存在子权限，无法删除");
        }

        await _permissionRepository.DeleteAsync(id);
        return true;
    }

    public async Task<List<PermissionResponseDto>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        var permissions = await _permissionRepository.GetPermissionsByRoleIdAsync(roleId);
        return permissions.Select(MapToDto).ToList();
    }

    private static PermissionResponseDto MapToDto(Permission entity)
    {
        return new PermissionResponseDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Type = entity.Type,
            Url = entity.Url,
            Method = entity.Method,
            ParentId = entity.ParentId,
            SortOrder = entity.SortOrder,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt
        };
    }

    private static PermissionResponseDto MapToDtoWithChildren(Permission entity)
    {
        var dto = MapToDto(entity);
        dto.Children = entity.Children?.Select(MapToDtoWithChildren).ToList();
        return dto;
    }
}