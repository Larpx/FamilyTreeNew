using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 操作日志服务接口，提供操作日志的记录和查询功能
/// </summary>
public interface IOperationLogService
{
    /// <summary>
    /// 记录操作日志
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="operationType">操作类型</param>
    /// <param name="module">操作模块</param>
    /// <param name="content">操作内容</param>
    /// <param name="ipAddress">IP地址</param>
    /// <param name="userAgent">用户代理</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误消息</param>
    Task LogAsync(Guid? adminId, string operationType, string module, string? content, string? ipAddress = null, string? userAgent = null, bool isSuccess = true, string? errorMessage = null);

    /// <summary>
    /// 分页获取操作日志列表
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    Task<PagedResult<OperationLog>> GetListAsync(int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 根据管理员ID分页获取操作日志
    /// </summary>
    /// <param name="adminId">管理员ID</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    Task<PagedResult<OperationLog>> GetByAdminIdAsync(Guid adminId, int pageIndex = 1, int pageSize = 20);
}
