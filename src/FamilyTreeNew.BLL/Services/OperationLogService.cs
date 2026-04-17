using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 操作日志服务接口。
/// 定义系统操作日志的记录和分页查询方法。
/// </summary>
public interface IOperationLogService
{
    /// <summary>
    /// 记录一条操作日志。
    /// </summary>
    Task LogAsync(Guid? adminId, string operationType, string module, string? content, string? ipAddress = null, string? userAgent = null, bool isSuccess = true, string? errorMessage = null);
    /// <summary>
    /// 分页获取操作日志列表。
    /// </summary>
    Task<PagedResult<OperationLog>> GetListAsync(int pageIndex = 1, int pageSize = 20);
    /// <summary>
    /// 分页获取指定管理员的操作日志。
    /// </summary>
    Task<PagedResult<OperationLog>> GetByAdminIdAsync(Guid adminId, int pageIndex = 1, int pageSize = 20);
}

/// <summary>
/// 操作日志服务实现。
/// 负责把日志实体写入数据库，并提供分页查询功能。
/// </summary>
public class OperationLogService : IOperationLogService
{
    private readonly IOperationLogRepository _repository;

    public OperationLogService(IOperationLogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 记录一条操作日志。
    /// </summary>
    public async Task LogAsync(
        Guid? adminId,
        string operationType,
        string module,
        string? content,
        string? ipAddress = null,
        string? userAgent = null,
        bool isSuccess = true,
        string? errorMessage = null)
    {
        var log = new OperationLog
        {
            AdminId = adminId,
            OperationType = operationType,
            Module = module,
            Content = content,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            OperationTime = DateTime.Now
        };

        await _repository.InsertAsync(log);
    }

    /// <summary>
    /// 分页获取操作日志列表。
    /// </summary>
    public async Task<PagedResult<OperationLog>> GetListAsync(int pageIndex = 1, int pageSize = 20)
    {
        var items = await _repository.GetListAsync(pageIndex, pageSize);
        var totalCount = await _repository.GetCountAsync();

        return new PagedResult<OperationLog>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 分页获取指定管理员的操作日志。
    /// </summary>
    public async Task<PagedResult<OperationLog>> GetByAdminIdAsync(Guid adminId, int pageIndex = 1, int pageSize = 20)
    {
        var items = await _repository.GetByAdminIdAsync(adminId, pageIndex, pageSize);
        var totalCount = await _repository.GetCountByAdminIdAsync(adminId);

        return new PagedResult<OperationLog>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }
}
