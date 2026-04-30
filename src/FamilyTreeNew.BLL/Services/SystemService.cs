using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace FamilyTreeNew.BLL.Services;

public interface ISystemService
{
    Task<DatabaseStatusDto> GetDatabaseStatusAsync();
}

public class SystemService : ISystemService
{
    private readonly SqlSugarContext _context;
    private readonly ILogger<SystemService> _logger;
    private readonly IConfiguration _configuration;

    public SystemService(
        SqlSugarContext context,
        ILogger<SystemService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<DatabaseStatusDto> GetDatabaseStatusAsync()
    {
        var status = new DatabaseStatusDto
        {
            CheckTime = DateTime.UtcNow
        };

        try
        {
            var db = _context.Db;

            var canConnect = await Task.Run(() => db.Ado.IsValidConnection());
            status.IsConnected = canConnect;

            if (!canConnect)
            {
                return status;
            }

            var versionInfo = await db.Ado.GetScalarAsync("SELECT VERSION()");
            status.ServerVersion = versionInfo?.ToString();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            status.DatabaseName = ConnectionStringHelper.ExtractDatabaseName(connectionString);

            var tables = await GetTableInfoAsync(db);
            status.Tables = tables;
            status.TotalRecords = tables.Sum(t => t.RecordCount);

            _logger.LogInformation("Database status check completed successfully");
        }
        catch (Exception ex)
        {
            status.IsConnected = false;
            status.ErrorMessage = "数据库连接异常，请查看系统日志";
            _logger.LogError(ex, "Error checking database status");
        }

        return status;
    }

    private async Task<List<TableInfoDto>> GetTableInfoAsync(ISqlSugarClient db)
    {
        var tables = new List<TableInfoDto>();

        var expectedTables = new List<string>
        {
            "Families",
            "FamilyMembers",
            "FamilyTrees",
            "Admins",
            "VerificationQuestions",
            "Albums",
            "Photos",
            "OperationLogs",
            "SystemSettings"
        };

        var dbTableInfos = await Task.Run(() => db.DbMaintenance.GetTableInfoList(isCache: false));
        var dbTableDict = dbTableInfos.ToDictionary(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase);

        foreach (var tableName in expectedTables)
        {
            try
            {
                if (!dbTableDict.TryGetValue(tableName, out var dbTableInfo))
                {
                    tables.Add(new TableInfoDto
                    {
                        TableName = tableName,
                        RecordCount = -1,
                        TableComment = "表不存在或无法访问"
                    });
                    continue;
                }

                var countObj = await db.Ado.GetScalarAsync($"SELECT COUNT(*) FROM `{tableName}`");

                var tableInfo = new TableInfoDto
                {
                    TableName = tableName,
                    RecordCount = Convert.ToInt64(countObj ?? 0),
                    TableComment = dbTableInfo.Description
                };

                tables.Add(tableInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Table {TableName} not found or error accessing it", tableName);
                tables.Add(new TableInfoDto
                {
                    TableName = tableName,
                    RecordCount = -1,
                    TableComment = "表不存在或无法访问"
                });
            }
        }

        return tables;
    }
}
