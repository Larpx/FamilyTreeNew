using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public interface IOperationLogService
{
    Task LogAsync(Guid? adminId, string operationType, string module, string? content, string? ipAddress = null, string? userAgent = null, bool isSuccess = true, string? errorMessage = null);
    Task<PagedResult<OperationLog>> GetListAsync(int pageIndex = 1, int pageSize = 20);
    Task<PagedResult<OperationLog>> GetByAdminIdAsync(Guid adminId, int pageIndex = 1, int pageSize = 20);
}

public class OperationLogService : IOperationLogService
{
    private readonly IOperationLogRepository _repository;

    public OperationLogService(IOperationLogRepository repository)
    {
        _repository = repository;
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
            OperationTime = DateTime.Now
        };

        await _repository.InsertAsync(log);
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
