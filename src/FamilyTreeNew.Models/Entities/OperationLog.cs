using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 操作日志实体类
/// 记录管理员在系统中的所有操作，用于审计追踪
/// </summary>
[SugarTable("OperationLogs")]
public class OperationLog
{
    /// <summary>
    /// 日志唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "日志ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 执行操作的管理员ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "管理员ID")]
    public Guid? AdminId { get; set; }

    /// <summary>
    /// 操作类型，如"创建"、"更新"、"删除"等
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "操作类型")]
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 操作模块，如"家谱管理"、"成员管理"等
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "操作模块")]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作详细内容描述
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "操作内容")]
    public string? Content { get; set; }

    /// <summary>
    /// 操作执行时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "操作时间")]
    public DateTime OperationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 操作者的IP地址
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "IP地址")]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 浏览器User-Agent信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "用户代理")]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 操作是否成功执行
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "是否成功")]
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// 操作失败时的错误信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "错误信息")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 导航属性：执行操作的管理员
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(AdminId))]
    public Admin? Admin { get; set; }
}
