using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.DAL.Repositories;

/// <summary>
/// 家族成员仓储接口
/// </summary>
public interface IFamilyMemberRepository
{
    /// <summary>
    /// 根据ID获取成员
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>成员实体</returns>
    Task<FamilyMember?> GetByIdAsync(Guid id);

    /// <summary>
    /// 根据ID获取成员（包含父成员信息）
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>成员实体</returns>
    Task<FamilyMember?> GetByIdWithParentAsync(Guid id);

    /// <summary>
    /// 判断成员是否存在
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>存在返回true</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// 判断成员是否存在于指定家谱中
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>存在返回true</returns>
    Task<bool> ExistsInFamilyTreeAsync(Guid id, Guid familyTreeId);

    /// <summary>
    /// 插入成员
    /// </summary>
    /// <param name="member">成员实体</param>
    /// <returns>受影响行数</returns>
    Task<int> InsertAsync(FamilyMember member);

    /// <summary>
    /// 更新成员
    /// </summary>
    /// <param name="member">成员实体</param>
    /// <returns>受影响行数</returns>
    Task<int> UpdateAsync(FamilyMember member);

    /// <summary>
    /// 删除成员
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>受影响行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 分页查询指定家谱的成员
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <param name="pageIndex">页码（从1开始）</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="keyword">关键词</param>
    /// <param name="generation">世代（可选）</param>
    /// <param name="parentId">父成员ID（可选）</param>
    /// <returns>分页数据和总记录数</returns>
    Task<(List<FamilyMember> Items, int TotalCount)> GetPagedByFamilyTreeAsync(
        Guid familyTreeId, int pageIndex, int pageSize, string? keyword, int? generation, Guid? parentId);

    /// <summary>
    /// 获取指定家谱的所有成员
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>成员列表</returns>
    Task<List<FamilyMember>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    /// <summary>
    /// 判断成员是否有子成员
    /// </summary>
    /// <param name="parentId">父成员ID</param>
    /// <returns>有子成员返回true</returns>
    Task<bool> HasChildrenAsync(Guid parentId);

    /// <summary>
    /// 获取指定父成员的子成员列表
    /// </summary>
    /// <param name="parentId">父成员ID</param>
    /// <returns>子成员列表</returns>
    Task<List<FamilyMember>> GetChildrenAsync(Guid parentId);

    /// <summary>
    /// 批量插入成员
    /// </summary>
    /// <param name="members">成员列表</param>
    /// <returns>受影响行数</returns>
    Task<int> BulkInsertAsync(List<FamilyMember> members);

    /// <summary>
    /// 根据父成员ID获取世代数
    /// </summary>
    /// <param name="parentId">父成员ID</param>
    /// <returns>世代数</returns>
    Task<int?> GetGenerationByParentIdAsync(Guid? parentId);

    /// <summary>
    /// 批量获取父成员姓名
    /// </summary>
    /// <param name="parentIds">父成员ID列表</param>
    /// <returns>成员ID与姓名的字典</returns>
    Task<Dictionary<Guid, string>> GetParentNamesAsync(List<Guid> parentIds);
}
