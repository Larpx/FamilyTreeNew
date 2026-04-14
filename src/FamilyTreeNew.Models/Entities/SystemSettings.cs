using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 系统设置实体类
/// 存储系统全局配置信息，通常只有一条记录
/// </summary>
[SugarTable("SystemSettings")]
public class SystemSettings
{
    /// <summary>
    /// 设置记录ID，自增主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 网站名称，显示在页面标题
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "网站名称")]
    public string SiteName { get; set; } = "家族族谱";

    /// <summary>
    /// 网站描述，用于SEO
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "网站描述")]
    public string? SiteDescription { get; set; }

    /// <summary>
    /// 网站Logo图片URL
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "网站Logo URL")]
    public string? LogoUrl { get; set; }

    /// <summary>
    /// 网站Favicon图标URL
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "网站Favicon URL")]
    public string? FaviconUrl { get; set; }

    /// <summary>
    /// 主题颜色，十六进制颜色值
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "主题颜色")]
    public string ThemeColor { get; set; } = "#1890ff";

    /// <summary>
    /// 是否在首页显示家谱统计数据
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否显示家谱统计")]
    public bool ShowStatistics { get; set; } = true;

    /// <summary>
    /// 是否允许未登录用户浏览家谱
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否允许游客浏览")]
    public bool AllowGuestBrowse { get; set; } = false;

    /// <summary>
    /// 访问家谱是否需要回答验证问题
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否需要验证问题")]
    public bool RequireVerification { get; set; } = true;

    /// <summary>
    /// 登录失败多少次后锁定账户
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "登录失败锁定次数")]
    public int MaxLoginAttempts { get; set; } = 5;

    /// <summary>
    /// 账户锁定持续时间（分钟）
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "账户锁定时间(分钟)")]
    public int LockoutDuration { get; set; } = 30;

    /// <summary>
    /// 用户会话超时时间（分钟）
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "会话超时时间(分钟)")]
    public int SessionTimeout { get; set; } = 120;

    /// <summary>
    /// 是否启用操作日志记录
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用操作日志")]
    public bool EnableOperationLog { get; set; } = true;

    /// <summary>
    /// 日志保留天数，超过天数的日志将被自动清理
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "日志保留天数")]
    public int LogRetentionDays { get; set; } = 90;

    /// <summary>
    /// 管理员联系邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "联系邮箱")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 管理员联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "联系电话")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系地址
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "联系地址")]
    public string? ContactAddress { get; set; }

    /// <summary>
    /// 页脚显示的自定义文本
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "页脚信息")]
    public string? FooterText { get; set; }

    /// <summary>
    /// 设置创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 设置最后更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdatedAt { get; set; }
}
