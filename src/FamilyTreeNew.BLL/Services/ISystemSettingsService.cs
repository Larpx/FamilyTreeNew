using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 系统设置服务接口，提供系统设置的查询和更新功能
/// </summary>
public interface ISystemSettingsService
{
    /// <summary>
    /// 获取系统设置
    /// </summary>
    Task<SystemSettingsDto?> GetSettingsAsync();

    /// <summary>
    /// 更新系统设置
    /// </summary>
    /// <param name="dto">系统设置更新数据</param>
    Task<SystemSettingsDto?> UpdateSettingsAsync(UpdateSystemSettingsDto dto);
}
