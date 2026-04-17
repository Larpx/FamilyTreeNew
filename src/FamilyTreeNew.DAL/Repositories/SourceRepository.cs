using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 来源仓储接口。
/// 负责按类型查询来源并获取启用状态的来源列表。
/// </summary>
public interface ISourceRepository : IBaseRepositoryGuid<Source>
{
    Task<List<Source>> GetByTypeAsync(string type);

    Task<List<Source>> GetEnabledSourcesAsync();
}