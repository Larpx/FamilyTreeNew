using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 角色服务。
/// 负责角色的分页查询、创建、更新、删除，以及角色权限的分配与读取。
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    /// <summary>
    /// 构造函数。
    /// 用于注入角色仓储和权限仓储。
    /// </summary>
    public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    /// <summary>
    /// 分页获取角色列表。
    /// 支持按关键字筛选角色名称和编码。
    /// </summary>
    public async Task<PagedResult<RoleResponseDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null)
    {
        var (items, totalCount) = await _roleRepository.GetPagedAsync(pageIndex, pageSize,
            keyword != null ? r => r.Name.Contains(keyword) || r.Code.Contains(keyword) : null);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<RoleResponseDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 根据 ID 获取角色。
    /// 会同时加载该角色的权限信息。
    /// </summary>
    public async Task<RoleResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _roleRepository.GetWithPermissionsAsync(id);
        return entity != null ? MapToDtoWithPermissions(entity) : null;
    }

    /// <summary>
    /// 根据编码获取角色。
    /// 编码通常用于程序内部做权限判断。
    /// </summary>
    public async Task<RoleResponseDto?> GetByCodeAsync(string code)
    {
        var entity = await _roleRepository.GetByCodeAsync(code);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 创建角色。
    /// 创建前会检查编码是否重复。
    /// </summary>
    public async Task<RoleResponseDto> CreateAsync(RoleCreateRequestDto dto)
    {
        var existingRole = await _roleRepository.GetByCodeAsync(dto.Code);
        if (existingRole != null)
        {
            throw new InvalidOperationException($"角色编码 '{dto.Code}' 已存在");
        }

        var entity = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            Code = dto.Code,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _roleRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新角色。
    /// 如果目标角色不存在，则返回 null。
    /// </summary>
    public async Task<RoleResponseDto?> UpdateAsync(Guid id, RoleUpdateRequestDto dto)
    {
        var entity = await _roleRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 删除角色。
    /// 仅在角色存在时执行删除。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _roleRepository.ExistsAsync(id)) return false;
        await _roleRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 为角色分配权限。
    /// 该方法会先清空旧关联，再插入新的角色权限关系。
    /// </summary>
    public async Task<bool> AssignPermissionsAsync(Guid roleId, List<Guid> permissionIds)
    {
        if (!await _roleRepository.ExistsAsync(roleId)) return false;

        using var db = _roleRepository.Db;
        await db.Ado.BeginTranAsync();

        try
        {
            await db.Deleteable<RolePermission>().Where(rp => rp.RoleId == roleId).ExecuteCommandAsync();

            if (permissionIds.Any())
            {
                var rolePermissions = permissionIds.Select(pid => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = pid,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await db.Insertable(rolePermissions).ExecuteCommandAsync();
            }

            await db.Ado.CommitTranAsync();
            return true;
        }
        catch
        {
            await db.Ado.RollbackTranAsync();
            throw;
        }
    }

    /// <summary>
    /// 根据管理员 ID 获取角色列表。
    /// 返回该管理员拥有的所有角色。
    /// </summary>
    public async Task<List<RoleResponseDto>> GetRolesByAdminIdAsync(Guid adminId)
    {
        var roles = await _roleRepository.GetRolesByAdminIdAsync(adminId);
        return roles.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 将角色实体转换为 DTO。
    /// 这是一个纯映射方法。
    /// </summary>
    private static RoleResponseDto MapToDto(Role entity)
    {
        return new RoleResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Code = entity.Code,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 将角色实体及其权限转换为 DTO。
    /// 用于角色详情或角色树的展示。
    /// </summary>
    private static RoleResponseDto MapToDtoWithPermissions(Role entity)
    {
        var dto = MapToDto(entity);
        dto.Permissions = entity.Permissions?.Select(p => new PermissionResponseDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Type = p.Type,
            Url = p.Url,
            Method = p.Method,
            ParentId = p.ParentId,
            SortOrder = p.SortOrder,
            IsEnabled = p.IsEnabled,
            CreatedAt = p.CreatedAt
        }).ToList();

        return dto;
    }
}