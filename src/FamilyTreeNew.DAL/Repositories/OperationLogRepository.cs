using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 操作日志仓储接口，继承自IBaseRepositoryGuid，提供操作日志的分页查询和统计功能
/// </summary>
public interface IOperationLogRepository : IBaseRepositoryGuid<OperationLog>
{
    /// <summary>
    /// 分页获取操作日志列表（按操作时间降序，包含管理员信息）
    /// </summary>
    /// <param name="pageIndex">页码（从1开始），默认1</param>
    /// <param name="pageSize">每页数量，默认20</param>
    Task<List<OperationLog>> GetListAsync(int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 根据管理员ID分页获取操作日志
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="pageIndex">页码（从1开始），默认1</param>
    /// <param name="pageSize">每页数量，默认20</param>
    Task<List<OperationLog>> GetByAdminIdAsync(Guid adminId, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 获取操作日志总数
    /// </summary>
    Task<int> GetCountAsync();

    /// <summary>
    /// 获取指定管理员的操作日志总数
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    Task<int> GetCountByAdminIdAsync(Guid adminId);
}

/// <summary>
/// 操作日志仓储实现，继承自BaseRepositoryGuid
/// </summary>
public class OperationLogRepository : BaseRepositoryGuid<OperationLog>, IOperationLogRepository
{
    public OperationLogRepository(SqlSugarContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<List<OperationLog>> GetListAsync(int pageIndex = 1, int pageSize = 20)
    {
        return await Db.Queryable<OperationLog>()
            .Includes(x => x.Admin)
            .OrderByDescending(x => x.OperationTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<OperationLog>> GetByAdminIdAsync(Guid adminId, int pageIndex = 1, int pageSize = 20)
    {
        return await Db.Queryable<OperationLog>()
            .Includes(x => x.Admin)
            .Where(x => x.AdminId == adminId)
            .OrderByDescending(x => x.OperationTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetCountAsync()
    {
        return await Db.Queryable<OperationLog>().CountAsync();
    }

    /// <inheritdoc/>
    public async Task<int> GetCountByAdminIdAsync(Guid adminId)
    {
        return await Db.Queryable<OperationLog>()
            .Where(x => x.AdminId == adminId)
            .CountAsync();
    }
}
