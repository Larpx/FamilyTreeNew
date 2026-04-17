using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Text;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 备份服务接口。
/// 定义数据库备份、恢复、列表和删除的能力。
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// 创建一个新的数据库备份文件。
    /// </summary>
    Task<BackupDto> CreateBackupAsync();
    /// <summary>
    /// 从指定备份文件恢复数据库。
    /// </summary>
    Task<RestoreDto> RestoreBackupAsync(string fileName);
    /// <summary>
    /// 获取当前所有备份文件列表。
    /// </summary>
    BackupListDto GetBackupList();
    /// <summary>
    /// 删除指定备份文件。
    /// </summary>
    bool DeleteBackup(string fileName);
}

/// <summary>
/// 备份服务实现。
/// 负责创建、恢复、列出和删除数据库备份文件。
/// </summary>
public class BackupService : IBackupService
{
    private readonly SqlSugarContext _context;
    private readonly ILogger<BackupService> _logger;
    private readonly string _backupDirectory;
    private readonly string _databaseName;

    public BackupService(
        SqlSugarContext context,
        IConfiguration configuration,
        ILogger<BackupService> logger)
    {
        _context = context;
        _logger = logger;
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        _databaseName = ConnectionStringHelper.ExtractDatabaseName(connectionString);
        _backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
        EnsureBackupDirectoryExists();
    }

    /// <summary>
    /// 创建一个新的数据库备份文件。
    /// 备份文件会保存在网站根目录下的 `wwwroot/backups` 中。
    /// </summary>
    public async Task<BackupDto> CreateBackupAsync()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"backup_{_databaseName}_{timestamp}.sql";
        var filePath = Path.Combine(_backupDirectory, fileName);

        if (!IsPathWithinBackupDirectory(filePath))
        {
            return new BackupDto
            {
                FileName = fileName,
                FilePath = filePath,
                IsSuccess = false,
                ErrorMessage = "无效的文件路径"
            };
        }

        try
        {
            var db = _context.Db;

            await Task.Run(() => db.DbMaintenance.BackupDataBase(_databaseName, filePath));

            var fileInfo = new FileInfo(filePath);
            _logger.LogInformation("Backup created successfully: {FileName}", fileName);

            return new BackupDto
            {
                FileName = fileName,
                FilePath = filePath,
                FileSize = fileInfo.Length,
                CreatedAt = DateTime.Now,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backup");
            return new BackupDto
            {
                FileName = fileName,
                FilePath = filePath,
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// 从指定备份文件恢复数据库。
    /// 在执行前会验证文件名和路径，避免路径穿越。
    /// </summary>
    public async Task<RestoreDto> RestoreBackupAsync(string fileName)
    {
        if (!IsValidBackupFileName(fileName)) return new RestoreDto { FileName = fileName, IsSuccess = false, ErrorMessage = "无效的备份文件名" };

        var filePath = Path.Combine(_backupDirectory, fileName);

        if (!IsPathWithinBackupDirectory(filePath)) return new RestoreDto { FileName = fileName, IsSuccess = false, ErrorMessage = "无效的文件路径" };

        if (!File.Exists(filePath))
        {
            return new RestoreDto
            {
                FileName = fileName,
                IsSuccess = false,
                ErrorMessage = "备份文件不存在"
            };
        }

        try
        {
            var sqlContent = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            var sqlStatements = ParseSqlStatements(sqlContent);

            var db = _context.Db;

            foreach (var sql in sqlStatements)
            {
                try
                {
                    await db.Ado.ExecuteCommandAsync(sql);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error executing SQL statement during restore, skipping: {Sql}", sql.Length > 100 ? sql[..100] : sql);
                }
            }

            _logger.LogInformation("Database restored successfully from: {FileName}", fileName);

            return new RestoreDto
            {
                FileName = fileName,
                IsSuccess = true,
                RestoredAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring backup");
            return new RestoreDto
            {
                FileName = fileName,
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// 获取备份文件列表。
    /// 会扫描备份目录下的所有 SQL 文件。
    /// </summary>
    public BackupListDto GetBackupList()
    {
        if (!Directory.Exists(_backupDirectory))
        {
            return new BackupListDto();
        }

        var files = Directory.GetFiles(_backupDirectory, "*.sql")
            .OrderByDescending(f => File.GetCreationTime(f))
            .ToList();

        var backups = files.Select(f =>
        {
            var fileInfo = new FileInfo(f);
            return new BackupDto
            {
                FileName = Path.GetFileName(f),
                FilePath = f,
                FileSize = fileInfo.Length,
                CreatedAt = fileInfo.CreationTime,
                IsSuccess = true
            };
        }).ToList();

        return new BackupListDto
        {
            Backups = backups,
            TotalCount = backups.Count
        };
    }

    /// <summary>
    /// 删除指定备份文件。
    /// 删除前会校验文件名和路径是否安全。
    /// </summary>
    public bool DeleteBackup(string fileName)
    {
        if (!IsValidBackupFileName(fileName)) return false;

        var filePath = Path.Combine(_backupDirectory, fileName);

        if (!IsPathWithinBackupDirectory(filePath)) return false;

        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            File.Delete(filePath);
            _logger.LogInformation("Backup deleted: {FileName}", fileName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backup: {FileName}", fileName);
            return false;
        }
    }

    /// <summary>
    /// 判断备份文件名是否合法。
    /// 主要防止路径穿越和非法字符。
    /// </summary>
    private static bool IsValidBackupFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        var pattern = @"^[a-zA-Z0-9_\-\.\s]+$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(fileName, pattern)) return false;
        if (fileName.Contains("..")) return false;
        return true;
    }

    /// <summary>
    /// 判断指定路径是否仍位于备份目录之内。
    /// </summary>
    private bool IsPathWithinBackupDirectory(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);
        var backupDirFullPath = Path.GetFullPath(_backupDirectory);
        return fullPath.StartsWith(backupDirFullPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 把 SQL 文件内容拆分成可逐条执行的语句列表。
    /// </summary>
    private static List<string> ParseSqlStatements(string sqlContent)
    {
        var statements = new List<string>();
        var currentStatement = new StringBuilder();
        var inQuote = false;
        var quoteChar = '\'';

        for (var i = 0; i < sqlContent.Length; i++)
        {
            var c = sqlContent[i];

            if (c == '\'' && !inQuote)
            {
                inQuote = true;
                quoteChar = c;
                currentStatement.Append(c);
            }
            else if (c == quoteChar && inQuote)
            {
                if (i + 1 < sqlContent.Length && sqlContent[i + 1] == quoteChar)
                {
                    currentStatement.Append(c);
                    currentStatement.Append(sqlContent[i + 1]);
                    i++;
                }
                else
                {
                    inQuote = false;
                    currentStatement.Append(c);
                }
            }
            else if (c == ';' && !inQuote)
            {
                var stmt = currentStatement.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(stmt) && !stmt.StartsWith("--"))
                {
                    statements.Add(stmt);
                }
                currentStatement.Clear();
            }
            else
            {
                currentStatement.Append(c);
            }
        }

        var lastStmt = currentStatement.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(lastStmt) && !lastStmt.StartsWith("--"))
        {
            statements.Add(lastStmt);
        }

        return statements;
    }

    private void EnsureBackupDirectoryExists()
    {
        if (!Directory.Exists(_backupDirectory))
        {
            Directory.CreateDirectory(_backupDirectory);
            _logger.LogInformation("Created backup directory: {Directory}", _backupDirectory);
        }
    }
}
