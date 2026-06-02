using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家谱服务接口
/// </summary>
public interface IFamilyTreeService
{
    /// <summary>
    /// 分页查询家谱列表
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页结果</returns>
    Task<PagedResult<FamilyTreeDto>> GetPagedAsync(FamilyTreeQueryDto query);

    /// <summary>
    /// 根据ID获取家谱详情
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>家谱DTO</returns>
    Task<FamilyTreeDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建家谱
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>创建的家谱DTO</returns>
    Task<FamilyTreeDto> CreateAsync(FamilyTreeCreateDto dto);

    /// <summary>
    /// 更新家谱
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新后的家谱DTO</returns>
    Task<FamilyTreeDto?> UpdateAsync(Guid id, FamilyTreeUpdateDto dto);

    /// <summary>
    /// 删除家谱
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>删除成功返回true</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 判断家谱是否存在
    /// </summary>
    /// <param name="id">家谱ID</param>
    /// <returns>存在返回true</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// 使家谱缓存失效
    /// </summary>
    /// <param name="familyTreeId">家谱ID（可选）</param>
    void InvalidateCache(Guid? familyTreeId = null);
}
