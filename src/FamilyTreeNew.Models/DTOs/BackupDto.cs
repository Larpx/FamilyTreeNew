namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 备份信息DTO
/// </summary>
public class BackupDto
{
    /// <summary>
    /// 备份文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 备份文件路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 是否备份成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误信息（备份失败时）
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 备份列表响应DTO
/// </summary>
public class BackupListDto
{
    /// <summary>
    /// 备份列表
    /// </summary>
    public List<BackupDto> Backups { get; set; } = new();

    /// <summary>
    /// 备份总数
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// 恢复结果DTO
/// </summary>
public class RestoreDto
{
    /// <summary>
    /// 恢复的文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 是否恢复成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误信息（恢复失败时）
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 恢复时间
    /// </summary>
    public DateTime RestoredAt { get; set; }
}

/// <summary>
/// 恢复请求DTO
/// </summary>
public class RestoreRequestDto
{
    /// <summary>
    /// 要恢复的备份文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
