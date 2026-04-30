using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.Entities;
using SqlSugar;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 管理员仓储接口，继承自IBaseRepositoryGuid，提供管理员账号查询和登录时间更新功能
/// </summary>
public interface IAdminRepository : IBaseRepositoryGuid<Admin>
{
    /// <summary>
    /// 根据用户名获取管理员
    /// </summary>
    /// <param name="username">用户名</param>
    Task<Admin?> GetByUsernameAsync(string username);

    /// <summary>
    /// 更新管理员最后登录时间
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateLastLoginTimeAsync(Guid adminId);

    /// <summary>
    /// 判断指定用户名的管理员是否存在
    /// </summary>
    /// <param name="username">用户名</param>
    Task<bool> ExistsByUsernameAsync(string username);

    /// <summary>
    /// 根据ID获取管理员
    /// </summary>
    /// <param name="id">管理员ID</param>
    new Task<Admin?> GetByIdAsync(Guid id);

    /// <summary>
    /// 插入管理员
    /// </summary>
    /// <param name="entity">管理员实体</param>
    new Task<int> InsertAsync(Admin entity);

    /// <summary>
    /// 更新管理员
    /// </summary>
    /// <param name="entity">管理员实体</param>
    new Task<int> UpdateAsync(Admin entity);

    /// <summary>
    /// 删除管理员
    /// </summary>
    /// <param name="id">管理员ID</param>
    new Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 判断管理员是否存在
    /// </summary>
    /// <param name="id">管理员ID</param>
    new Task<bool> ExistsAsync(Guid id);
}

/// <summary>
/// 管理员仓储实现，继承自BaseRepositoryGuid
/// </summary>
public class AdminRepository : BaseRepositoryGuid<Admin>, IAdminRepository
{
    public AdminRepository(SqlSugarContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Admin?> GetByUsernameAsync(string username)
    {
        return await Db.Queryable<Admin>()
            .FirstAsync(a => a.Username == username);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateLastLoginTimeAsync(Guid adminId)
    {
        return await Db.Updateable<Admin>()
            .SetColumns(a => a.LastLoginAt == DateTime.UtcNow)
            .Where(a => a.Id == adminId)
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await Db.Queryable<Admin>()
            .AnyAsync(a => a.Username == username);
    }

    /// <inheritdoc/>
    public new Task<Admin?> GetByIdAsync(Guid id)
    {
        return base.GetByIdAsync(id);
    }

    /// <inheritdoc/>
    public new Task<int> InsertAsync(Admin entity)
    {
        return base.InsertAsync(entity);
    }

    /// <inheritdoc/>
    public new Task<int> UpdateAsync(Admin entity)
    {
        return base.UpdateAsync(entity);
    }

    /// <inheritdoc/>
    public new Task<int> DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public new Task<bool> ExistsAsync(Guid id)
    {
        return base.ExistsAsync(id);
    }
}
