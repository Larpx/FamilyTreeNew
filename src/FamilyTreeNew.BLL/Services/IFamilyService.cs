using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 家族服务接口，提供家族的增删改查功能
/// </summary>
public interface IFamilyService
{
    /// <summary>
    /// 获取所有家族列表
    /// </summary>
    Task<List<FamilyResponseDto>> GetAllFamiliesAsync();

    /// <summary>
    /// 根据ID获取家族详情
    /// </summary>
    /// <param name="id">家族ID</param>
    Task<FamilyResponseDto?> GetFamilyByIdAsync(int id);

    /// <summary>
    /// 创建家族
    /// </summary>
    /// <param name="dto">家族创建数据</param>
    Task<FamilyResponseDto> CreateFamilyAsync(FamilyCreateRequestDto dto);

    /// <summary>
    /// 更新家族信息
    /// </summary>
    /// <param name="id">家族ID</param>
    /// <param name="dto">家族更新数据</param>
    Task<FamilyResponseDto?> UpdateFamilyAsync(int id, FamilyUpdateRequestDto dto);

    /// <summary>
    /// 删除家族
    /// </summary>
    /// <param name="id">家族ID</param>
    Task<bool> DeleteFamilyAsync(int id);
}
