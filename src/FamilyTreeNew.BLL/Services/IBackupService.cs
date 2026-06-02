using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 备份服务接口，提供数据库备份和恢复功能
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// 创建数据库备份
    /// </summary>
    Task<BackupDto> CreateBackupAsync();

    /// <summary>
    /// 从备份文件恢复数据库
    /// </summary>
    /// <param name="fileName">备份文件名</param>
    Task<RestoreDto> RestoreBackupAsync(string fileName);

    /// <summary>
    /// 获取备份文件列表
    /// </summary>
    BackupListDto GetBackupList();

    /// <summary>
    /// 删除备份文件
    /// </summary>
    /// <param name="fileName">备份文件名</param>
    bool DeleteBackup(string fileName);
}
