using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 菜单仓储接口。
/// 负责获取树形菜单、启用菜单等数据访问能力。
/// </summary>
public interface IMenuRepository : IBaseRepositoryGuid<Menu>
{
    Task<List<Menu>> GetAllWithChildrenAsync();

    Task<List<Menu>> GetEnabledMenusAsync();
}