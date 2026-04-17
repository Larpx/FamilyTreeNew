using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 权限服务。
/// 负责权限树的查询、权限的新增编辑删除，以及角色权限关联查询。
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    /// <summary>
    /// 构造函数。
    /// 用于注入权限仓储。
    /// </summary>
    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    /// <summary>
    /// 获取全部权限。
    /// 返回扁平列表，适合基础管理页面。
    /// </summary>
    public async Task<List<PermissionResponseDto>> GetAllAsync()
    {
        var entities = await _permissionRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取树形权限。
    /// 适合权限管理界面按父子层级展示。
    /// </summary>
    public async Task<List<PermissionResponseDto>> GetAllWithTreeAsync()
    {
        var entities = await _permissionRepository.GetAllWithChildrenAsync();
        return entities.Select(MapToDtoWithChildren).ToList();
    }

    /// <summary>
    /// 根据 ID 获取权限。
    /// 找不到时返回 null。
    /// </summary>
    public async Task<PermissionResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _permissionRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 根据编码获取权限。
    /// 编码通常用于程序内部判断当前用户是否有权限。
    /// </summary>
    public async Task<PermissionResponseDto?> GetByCodeAsync(string code)
    {
        var entity = await _permissionRepository.GetByCodeAsync(code);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 创建权限。
    /// 创建前会先检查编码是否重复。
    /// </summary>
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

    /// <summary>
    /// 更新权限。
    /// 如果权限不存在，则返回 null。
    /// </summary>
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

    /// <summary>
    /// 删除权限。
    /// 如果存在子权限，则拒绝删除。
    /// </summary>
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

    /// <summary>
    /// 查询某个角色拥有的权限。
    /// 返回结果会转换为前端可直接展示的 DTO。
    /// </summary>
    public async Task<List<PermissionResponseDto>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        var permissions = await _permissionRepository.GetPermissionsByRoleIdAsync(roleId);
        return permissions.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 将权限实体转换为 DTO。
    /// 这是一个纯映射方法。
    /// </summary>
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

    /// <summary>
    /// 将权限实体及其子节点递归转换为 DTO。
    /// 用于构造树形权限列表。
    /// </summary>
    private static PermissionResponseDto MapToDtoWithChildren(Permission entity)
    {
        var dto = MapToDto(entity);
        dto.Children = entity.Children?.Select(MapToDtoWithChildren).ToList();
        return dto;
    }
}