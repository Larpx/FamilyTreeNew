using FamilyTreeNew.DAL.Context;
using SqlSugar;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 基础仓储接口（int类型主键）
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IBaseRepository<T> where T : class, new()
{
    /// <summary>
    /// SqlSugar客户端
    /// </summary>
    ISqlSugarClient Db { get; }

    /// <summary>
    /// 获取所有记录
    /// </summary>
    /// <returns>实体列表</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// 根据ID获取记录
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns>实体对象，不存在返回null</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// 插入新记录
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响行数</returns>
    Task<int> InsertAsync(T entity);

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(T entity);

    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteAsync(int id);
}

/// <summary>
/// 基础仓储实现（int类型主键）
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
{
    protected readonly SqlSugarContext _context;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public BaseRepository(SqlSugarContext context)
    {
        _context = context;
    }

    /// <summary>
    /// SqlSugar客户端
    /// </summary>
    public ISqlSugarClient Db => _context.Db;

    /// <summary>
    /// 获取所有记录
    /// </summary>
    /// <returns>实体列表</returns>
    public async Task<List<T>> GetAllAsync()
    {
        return await Db.Queryable<T>().ToListAsync();
    }

    /// <summary>
    /// 根据ID获取记录
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns>实体对象，不存在返回null</returns>
    public async Task<T?> GetByIdAsync(int id)
    {
        return await Db.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 插入新记录
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响行数</returns>
    public async Task<int> InsertAsync(T entity)
    {
        return await Db.Insertable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新记录
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>受影响行数</returns>
    public async Task<int> UpdateAsync(T entity)
    {
        return await Db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns>受影响行数</returns>
    public async Task<int> DeleteAsync(int id)
    {
        return await Db.Deleteable<T>().In(id).ExecuteCommandAsync();
    }
}
