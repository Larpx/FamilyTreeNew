using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;
using SqlSugar;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 家族成员仓储实现
/// </summary>
public class FamilyMemberRepository : BaseRepositoryGuid<FamilyMember>, IFamilyMemberRepository
{
    public FamilyMemberRepository(SqlSugarContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<FamilyMember?> GetByIdWithParentAsync(Guid id)
    {
        return await Db.Queryable<FamilyMember>()
            .Where(m => m.Id == id)
            .Includes(m => m.Parent)
            .FirstAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsInFamilyTreeAsync(Guid id, Guid familyTreeId)
    {
        return await Db.Queryable<FamilyMember>()
            .Where(m => m.Id == id && m.FamilyTreeId == familyTreeId)
            .AnyAsync();
    }

    /// <inheritdoc/>
    public async Task<(List<FamilyMember> Items, int TotalCount)> GetPagedByFamilyTreeAsync(
        Guid familyTreeId, int pageIndex, int pageSize, string? keyword, int? generation, Guid? parentId)
    {
        var query = Db.Queryable<FamilyMember>()
            .Where(m => m.FamilyTreeId == familyTreeId)
            .WhereIF(!string.IsNullOrWhiteSpace(keyword), m =>
                m.Surname.Contains(keyword!) ||
                m.FirstName.Contains(keyword!) ||
                (m.Alias != null && m.Alias.Contains(keyword!)))
            .WhereIF(generation.HasValue, m => m.Generation == generation!.Value)
            .WhereIF(parentId.HasValue, m => m.ParentId == parentId!.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(m => m.CreatedAt, OrderByType.Desc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Includes(m => m.Parent)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<List<FamilyMember>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        return await Db.Queryable<FamilyMember>()
            .Where(m => m.FamilyTreeId == familyTreeId)
            .Includes(m => m.Parent)
            .OrderBy(m => m.Generation, OrderByType.Asc)
            .OrderBy(m => m.CreatedAt, OrderByType.Asc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> HasChildrenAsync(Guid parentId)
    {
        return await Db.Queryable<FamilyMember>()
            .Where(m => m.ParentId == parentId)
            .AnyAsync();
    }

    /// <inheritdoc/>
    public async Task<int> BulkInsertAsync(List<FamilyMember> members)
    {
        return await Db.Insertable(members).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int?> GetGenerationByParentIdAsync(Guid? parentId)
    {
        if (!parentId.HasValue)
        {
            return null;
        }

        return await Db.Queryable<FamilyMember>()
            .Where(m => m.Id == parentId.Value)
            .Select(m => m.Generation)
            .FirstAsync();
    }

    /// <inheritdoc/>
    public async Task<Dictionary<Guid, string>> GetParentNamesAsync(List<Guid> parentIds)
    {
        if (parentIds == null || parentIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var distinctIds = parentIds.Distinct().ToList();
        var parents = await Db.Queryable<FamilyMember>()
            .Where(m => distinctIds.Contains(m.Id))
            .Select(m => new { m.Id, FullName = m.Surname + m.FirstName })
            .ToListAsync();

        return parents.ToDictionary(p => p.Id, p => p.FullName);
    }
}
