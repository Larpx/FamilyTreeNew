namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 数据库状态响应DTO
/// </summary>
public class DatabaseStatusDto
{
    /// <summary>
    /// 数据库连接是否正常
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// 连接错误信息（连接失败时）
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 数据库服务器版本
    /// </summary>
    public string? ServerVersion { get; set; }

    /// <summary>
    /// 当前数据库名称
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// 数据库表信息列表
    /// </summary>
    public List<TableInfoDto> Tables { get; set; } = new();

    /// <summary>
    /// 表总数
    /// </summary>
    public int TotalTables => Tables.Count;

    /// <summary>
    /// 总记录数
    /// </summary>
    public long TotalRecords { get; set; }

    /// <summary>
    /// 数据库大小（字节）
    /// </summary>
    public long DatabaseSize { get; set; }

    /// <summary>
    /// 检查时间
    /// </summary>
    public DateTime CheckTime { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 数据表信息DTO
/// </summary>
public class TableInfoDto
{
    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 记录数
    /// </summary>
    public long RecordCount { get; set; }

    /// <summary>
    /// 表注释
    /// </summary>
    public string? TableComment { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}
