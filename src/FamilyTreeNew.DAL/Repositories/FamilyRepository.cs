using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 家族仓储接口。
/// 负责封装家族相关的数据访问方法，例如带成员数量的查询。
/// </summary>
public interface IFamilyRepository : IBaseRepository<Family>
{
    Task<List<Family>> GetFamiliesWithMemberCountAsync();
}

/// <summary>
/// 家族仓储实现。
/// 提供家族表的基础增删改查功能以及额外的业务查询。
/// </summary>
public class FamilyRepository : BaseRepository<Family>, IFamilyRepository
{
    public FamilyRepository(Context.SqlSugarContext context) : base(context)
    {
    }

    public async Task<List<Family>> GetFamiliesWithMemberCountAsync()
    {
        return await Db.Queryable<Family>()
            .OrderBy(f => f.CreatedAt, SqlSugar.OrderByType.Desc)
            .ToListAsync();
    }
}
