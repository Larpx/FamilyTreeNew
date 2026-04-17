using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface ISourceRepository : IBaseRepositoryGuid<Source>
{
    Task<List<Source>> GetByTypeAsync(string type);

    Task<List<Source>> GetEnabledSourcesAsync();
}