using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 管理员管理服务接口，提供管理员的增删改查功能
/// </summary>
public interface IAdminManagementService
{
    /// <summary>
    /// 分页获取管理员列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="keyword">搜索关键词</param>
    Task<PagedResult<AdminDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);

    /// <summary>
    /// 根据ID获取管理员详情
    /// </summary>
    /// <param name="id">管理员ID</param>
    Task<AdminDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建管理员
    /// </summary>
    /// <param name="dto">管理员创建数据</param>
    Task<AdminDto> CreateAsync(CreateAdminDto dto);

    /// <summary>
    /// 更新管理员信息
    /// </summary>
    /// <param name="id">管理员ID</param>
    /// <param name="dto">管理员更新数据</param>
    Task<AdminDto?> UpdateAsync(Guid id, UpdateAdminDto dto);

    /// <summary>
    /// 删除管理员
    /// </summary>
    /// <param name="id">管理员ID</param>
    Task<bool> DeleteAsync(Guid id);
}
