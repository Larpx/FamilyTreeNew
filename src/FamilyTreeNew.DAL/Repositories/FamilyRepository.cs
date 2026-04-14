using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

public interface IFamilyRepository : IBaseRepository<Family>
{
    Task<List<Family>> GetFamiliesWithMemberCountAsync();
}

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
