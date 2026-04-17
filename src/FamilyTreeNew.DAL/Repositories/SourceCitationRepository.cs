using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface ISourceCitationRepository : IBaseRepositoryGuid<SourceCitation>
{
    Task<List<SourceCitation>> GetBySourceIdAsync(Guid sourceId);

    Task<List<SourceCitation>> GetByTargetAsync(string targetType, Guid targetId);
}