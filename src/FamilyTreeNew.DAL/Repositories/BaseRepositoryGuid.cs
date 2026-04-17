using SqlSugar;
using FamilyTreeNew.DAL.Context;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 基于Guid主键的泛型仓储基类，提供通用的CRUD操作和分页查询功能
/// </summary>
/// <typeparam name="T">实体类型，必须具有无参构造函数</typeparam>
public interface IBaseRepositoryGuid<T> where T : class, new()
{
    /// <summary>
    /// SqlSugar数据库客户端
    /// </summary>
    ISqlSugarClient Db { get; }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体Guid主键</param>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// 分页查询实体
    /// </summary>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="predicate">查询条件表达式，可为null</param>
    /// <returns>包含分页数据和总记录数的元组</returns>
    Task<(List<T> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <param name="entity">待插入的实体</param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertAsync(T entity);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">待更新的实体</param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateAsync(T entity);

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">待删除实体的Guid主键</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 判断指定ID的实体是否存在
    /// </summary>
    /// <param name="id">实体Guid主键</param>
    Task<bool> ExistsAsync(Guid id);
}

/// <summary>
/// 基于Guid主键的泛型仓储基类实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class BaseRepositoryGuid<T> : IBaseRepositoryGuid<T> where T : class, new()
{
    /// <summary>
    /// SqlSugar数据库上下文
    /// </summary>
    protected readonly SqlSugarContext _context;

    public BaseRepositoryGuid(SqlSugarContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public ISqlSugarClient Db => _context.Db;

    /// <inheritdoc/>
    public async Task<List<T>> GetAllAsync()
    {
        return await Db.Queryable<T>().ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Db.Queryable<T>().InSingleAsync(id);
    }

    /// <inheritdoc/>
    public async Task<(List<T> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null)
    {
        var query = Db.Queryable<T>();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query.CountAsync();
        var items = await query.ToPageListAsync(pageIndex, pageSize);

        return (items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<int> InsertAsync(T entity)
    {
        return await Db.Insertable(entity).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(T entity)
    {
        return await Db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(Guid id)
    {
        return await Db.Deleteable<T>().In(id).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Db.Queryable<T>().In(id).AnyAsync();
    }
}
