using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Logging;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 操作日志服务
/// 记录和查询管理员操作日志
/// </summary>
public class OperationLogService : IOperationLogService
{
    private readonly IOperationLogRepository _repository;
    private readonly ILogger<OperationLogService> _logger;

    public OperationLogService(IOperationLogRepository repository, ILogger<OperationLogService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

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
            OperationTime = DateTime.UtcNow
        };

        await _repository.InsertAsync(log);
        _logger.LogInformation("记录操作日志，管理员: {AdminId}，操作: {OperationType}，模块: {Module}", adminId, operationType, module);
    }

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
