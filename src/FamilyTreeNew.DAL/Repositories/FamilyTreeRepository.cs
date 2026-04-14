using FamilyTreeNew.Models.Entities;
using SqlSugar;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 家谱仓储接口
/// </summary>
public interface IFamilyTreeRepository : IBaseRepositoryGuid<FamilyTree>
{
    /// <summary>
    /// 分页查询家谱（包含成员数量统计）
    /// </summary>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="keyword">关键词（搜索家谱名称）</param>
    /// <param name="isEnabled">是否启用（可选）</param>
    /// <returns>分页数据和总记录数</returns>
    Task<(List<FamilyTree> Items, int TotalCount)> GetPagedWithMemberCountAsync(int pageIndex, int pageSize, string? keyword, bool? isEnabled);

    /// <summary>
    /// 根据ID获取家谱（包含成员数量统计）
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>家谱实体</returns>
    Task<FamilyTree?> GetByIdWithMemberCountAsync(Guid id);

    /// <summary>
    /// 获取家谱成员数量
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>成员数量</returns>
    Task<int> GetMemberCountAsync(Guid familyTreeId);

    /// <summary>
    /// 批量获取多个家谱的成员数量
    /// </summary>
    /// <param name="familyTreeIds">家谱ID列表</param>
    /// <returns>家谱ID与成员数量的字典</returns>
    Task<Dictionary<Guid, int>> GetMemberCountsAsync(List<Guid> familyTreeIds);
}

/// <summary>
/// 家谱仓储实现
/// </summary>
public class FamilyTreeRepository : BaseRepositoryGuid<FamilyTree>, IFamilyTreeRepository
{
    public FamilyTreeRepository(Context.SqlSugarContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<(List<FamilyTree> Items, int TotalCount)> GetPagedWithMemberCountAsync(int pageIndex, int pageSize, string? keyword, bool? isEnabled)
    {
        var query = Db.Queryable<FamilyTree>()
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), ft => ft.Name.Contains(keyword!))
            .WhereIF(isEnabled.HasValue, ft => ft.IsEnabled == isEnabled!.Value)
            .OrderBy(ft => ft.CreatedAt, OrderByType.Desc);

        var totalCount = await query.CountAsync();
        var items = await query.ToPageListAsync(pageIndex, pageSize);

        return (items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<FamilyTree?> GetByIdWithMemberCountAsync(Guid id)
    {
        return await Db.Queryable<FamilyTree>()
            .InSingleAsync(id);
    }

    /// <inheritdoc/>
    public async Task<int> GetMemberCountAsync(Guid familyTreeId)
    {
        return await Db.Queryable<FamilyMember>()
            .Where(fm => fm.FamilyTreeId == familyTreeId)
            .CountAsync();
    }

    /// <inheritdoc/>
    public async Task<Dictionary<Guid, int>> GetMemberCountsAsync(List<Guid> familyTreeIds)
    {
        if (familyTreeIds == null || familyTreeIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        var distinctIds = familyTreeIds.Distinct().ToList();
        var counts = await Db.Queryable<FamilyMember>()
            .Where(fm => distinctIds.Contains(fm.FamilyTreeId))
            .GroupBy(fm => fm.FamilyTreeId)
            .Select(fm => new { FamilyTreeId = fm.FamilyTreeId, Count = SqlFunc.AggregateCount(fm.Id) })
            .ToListAsync();

        var result = distinctIds.ToDictionary(id => id, _ => 0);
        foreach (var count in counts)
        {
            result[count.FamilyTreeId] = count.Count;
        }

        return result;
    }
}
