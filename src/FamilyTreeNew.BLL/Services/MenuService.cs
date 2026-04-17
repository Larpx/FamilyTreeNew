using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 菜单服务。
/// 负责把菜单仓储的数据转换为前端需要的树形 DTO，并处理新增、编辑和删除菜单的业务规则。
/// </summary>
public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    /// <summary>
    /// 构造函数。
    /// 通过依赖注入获取菜单仓储。
    /// </summary>
    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    /// <summary>
    /// 获取全部菜单。
    /// 返回扁平结构，适合列表页或基础数据展示。
    /// </summary>
    public async Task<List<MenuResponseDto>> GetAllAsync()
    {
        var entities = await _menuRepository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// 获取树形菜单。
    /// 适合菜单管理页面，能直接显示父子层级关系。
    /// </summary>
    public async Task<List<MenuResponseDto>> GetAllWithTreeAsync()
    {
        var entities = await _menuRepository.GetAllWithChildrenAsync();
        return entities.Select(MapToDtoWithChildren).ToList();
    }

    /// <summary>
    /// 获取启用中的菜单。
    /// 常用于前台导航栏或登录后的可见菜单。
    /// </summary>
    public async Task<List<MenuResponseDto>> GetEnabledMenusAsync()
    {
        var entities = await _menuRepository.GetEnabledMenusAsync();
        return entities.Select(MapToDtoWithChildren).ToList();
    }

    /// <summary>
    /// 根据 ID 获取菜单。
    /// 找不到时返回 null。
    /// </summary>
    public async Task<MenuResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _menuRepository.GetByIdAsync(id);
        return entity != null ? MapToDto(entity) : null;
    }

    /// <summary>
    /// 创建菜单。
    /// 会先组装实体，再保存到数据库。
    /// </summary>
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
            CreatedAt = DateTime.UtcNow
        };

        await _menuRepository.InsertAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 更新菜单。
    /// 如果目标菜单不存在，则返回 null。
    /// </summary>
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
        entity.UpdatedAt = DateTime.UtcNow;

        await _menuRepository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    /// <summary>
    /// 删除菜单。
    /// 如果存在子菜单，则拒绝删除，避免破坏树结构。
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _menuRepository.ExistsAsync(id)) return false;

        var allMenus = await _menuRepository.GetAllAsync();
        if (allMenus.Any(m => m.ParentId == id))
        {
            throw new InvalidOperationException("该菜单下存在子菜单，无法删除");
        }

        await _menuRepository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 将菜单实体转换为 DTO。
    /// 这是一个纯映射方法，不会访问数据库。
    /// </summary>
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

    /// <summary>
    /// 将菜单实体及其子节点递归转换为 DTO。
    /// 用于生成树形菜单结构。
    /// </summary>
    private static MenuResponseDto MapToDtoWithChildren(Menu entity)
    {
        var dto = MapToDto(entity);
        dto.Children = entity.Children?.Select(MapToDtoWithChildren).ToList();
        return dto;
    }
}