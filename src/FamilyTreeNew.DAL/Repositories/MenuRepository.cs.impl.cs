using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

public class MenuRepository : BaseRepositoryGuid<Menu>, IMenuRepository
{
    public MenuRepository(SqlSugarContext context) : base(context) { }

    public async Task<List<Menu>> GetAllWithChildrenAsync()
    {
        var menus = await Db.Queryable<Menu>()
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        var rootMenus = menus.Where(m => m.ParentId == null).ToList();
        BuildMenuTree(rootMenus, menus);

        return rootMenus;
    }

    public async Task<List<Menu>> GetEnabledMenusAsync()
    {
        var menus = await Db.Queryable<Menu>()
            .Where(m => m.IsEnabled)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        var rootMenus = menus.Where(m => m.ParentId == null).ToList();
        BuildMenuTree(rootMenus, menus);

        return rootMenus;
    }

    private void BuildMenuTree(List<Menu> parents, List<Menu> allMenus)
    {
        foreach (var parent in parents)
        {
            parent.Children = allMenus
                .Where(m => m.ParentId == parent.Id && m.IsVisible)
                .ToList();
            
            if (parent.Children?.Count > 0)
            {
                BuildMenuTree(parent.Children, allMenus);
            }
        }
    }
}