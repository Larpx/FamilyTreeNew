using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IMenuRepository : IBaseRepositoryGuid<Menu>
{
    Task<List<Menu>> GetAllWithChildrenAsync();
    
    Task<List<Menu>> GetEnabledMenusAsync();
}