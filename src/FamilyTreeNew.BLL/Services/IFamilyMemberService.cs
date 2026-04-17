using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家族成员服务接口
/// </summary>
public interface IFamilyMemberService
{
    /// <summary>
    /// 分页查询家族成员
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页结果</returns>
    Task<PagedResult<FamilyMemberDto>> GetPagedAsync(FamilyMemberQueryDto query);

    /// <summary>
    /// 根据ID获取家族成员详情
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>成员DTO</returns>
    Task<FamilyMemberDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建家族成员
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>创建的成员DTO</returns>
    Task<FamilyMemberDto> CreateAsync(FamilyMemberCreateDto dto);

    /// <summary>
    /// 更新家族成员
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新后的成员DTO</returns>
    Task<FamilyMemberDto?> UpdateAsync(Guid id, FamilyMemberUpdateDto dto);

    /// <summary>
    /// 删除家族成员
    /// </summary>
    /// <param name="id">成员ID</param>
    /// <returns>删除成功返回true</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 获取指定家谱的所有成员
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <returns>成员列表</returns>
    Task<List<FamilyMemberDto>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    /// <summary>
    /// 计算新成员的世代数
    /// </summary>
    /// <param name="parentId">父成员ID（可为null）</param>
    /// <returns>世代数</returns>
    Task<int> CalculateGenerationAsync(Guid? parentId);
}
