using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

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

    public async Task<RoleResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _roleRepository.GetWithPermissionsAsync(id);
        return entity != null ? MapToDtoWithPermissions(entity) : null;
    }

    public async Task<RoleResponseDto?> GetByCodeAsync(string code)
    {
        var entity = await _roleRepository.GetByCodeAsync(code);
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<RoleResponseDto> CreateAsync(RoleCreateRequestDto dto)
    {
        var entity = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            Code = dto.Code,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.Now
        };

        await _roleRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    public async Task<RoleResponseDto?> UpdateAsync(Guid id, RoleUpdateRequestDto dto)
    {
        var entity = await _roleRepository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.IsEnabled = dto.IsEnabled;
        entity.UpdatedAt = DateTime.Now;

        await _roleRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _roleRepository.ExistsAsync(id)) return false;
        await _roleRepository.DeleteAsync(id);
        return true;
    }

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
                    CreatedAt = DateTime.Now
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

    public async Task<List<RoleResponseDto>> GetRolesByAdminIdAsync(Guid adminId)
    {
        var roles = await _roleRepository.GetRolesByAdminIdAsync(adminId);
        return roles.Select(MapToDto).ToList();
    }

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