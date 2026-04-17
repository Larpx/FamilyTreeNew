using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 来源引用仓储接口。
/// 负责查询某个来源的所有引用，以及查询某个目标对象被哪些来源引用。
/// </summary>
public interface ISourceCitationRepository : IBaseRepositoryGuid<SourceCitation>
{
    Task<List<SourceCitation>> GetBySourceIdAsync(Guid sourceId);

    Task<List<SourceCitation>> GetByTargetAsync(string targetType, Guid targetId);
}