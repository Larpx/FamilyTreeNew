using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 系统服务接口，提供系统状态监控功能
/// </summary>
public interface ISystemService
{
    /// <summary>
    /// 获取数据库状态信息
    /// </summary>
    Task<DatabaseStatusDto> GetDatabaseStatusAsync();
}
