using System.Text;
using FamilyTreeNew.BLL.Helpers;
using FamilyTreeNew.DAL.Context;
using FamilyTreeNew.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace FamilyTreeNew.BLL.Services;

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

    public async Task<BackupDto> CreateBackupAsync()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var fileName = $"backup_{_databaseName}_{timestamp}.sql";
        var filePath = Path.Combine(_backupDirectory, fileName);

        if (!IsPathWithinBackupDirectory(filePath))
        {
            return new BackupDto
            {
                FileName = fileName,
                FilePath = fileName,
                IsSuccess = false,
                ErrorMessage = "无效的文件路径"
            };
        }

        try
        {
            var db = _context.Db;

            await Task.Run(() => db.DbMaintenance.BackupDataBase(_databaseName, filePath));

            var fileInfo = new FileInfo(filePath);
            _logger.LogInformation("备份创建成功: {FileName}", fileName);

            return new BackupDto
            {
                FileName = fileName,
                FilePath = fileName,
                FileSize = fileInfo.Length,
                CreatedAt = DateTime.UtcNow,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建备份失败");
            return new BackupDto
            {
                FileName = fileName,
                FilePath = fileName,
                IsSuccess = false,
                ErrorMessage = "创建备份失败，请查看系统日志"
            };
        }
    }

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
                    _logger.LogWarning(ex, "恢复备份时执行SQL语句失败，已跳过: {Sql}", sql.Length > 100 ? sql[..100] : sql);
                }
            }

            _logger.LogInformation("数据库恢复成功: {FileName}", fileName);

            return new RestoreDto
            {
                FileName = fileName,
                IsSuccess = true,
                RestoredAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "恢复备份失败");
            return new RestoreDto
            {
                FileName = fileName,
                IsSuccess = false,
                ErrorMessage = "恢复备份失败，请查看系统日志"
            };
        }
    }

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
                FilePath = Path.GetFileName(f),
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
            _logger.LogInformation("备份已删除: {FileName}", fileName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除备份失败: {FileName}", fileName);
            return false;
        }
    }

    private static bool IsValidBackupFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        var pattern = @"^[a-zA-Z0-9_\-\.\s]+$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(fileName, pattern)) return false;
        if (fileName.Contains("..")) return false;
        return true;
    }

    private bool IsPathWithinBackupDirectory(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);
        var backupDirFullPath = Path.GetFullPath(_backupDirectory);
        return fullPath.StartsWith(backupDirFullPath, StringComparison.OrdinalIgnoreCase);
    }

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
            _logger.LogInformation("已创建备份目录: {Directory}", _backupDirectory);
        }
    }
}
