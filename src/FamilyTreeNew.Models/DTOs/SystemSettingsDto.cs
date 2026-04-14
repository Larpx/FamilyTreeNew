using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 系统设置响应DTO
/// </summary>
public class SystemSettingsDto
{
    /// <summary>
    /// 设置记录ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 网站名称
    /// </summary>
    [Required(ErrorMessage = "网站名称不能为空")]
    [StringLength(100, ErrorMessage = "网站名称不能超过100个字符")]
    public string SiteName { get; set; } = "家族族谱";

    /// <summary>
    /// 网站描述（用于SEO）
    /// </summary>
    [StringLength(500, ErrorMessage = "网站描述不能超过500个字符")]
    public string? SiteDescription { get; set; }

    /// <summary>
    /// 网站Logo图片URL
    /// </summary>
    [StringLength(200, ErrorMessage = "Logo URL不能超过200个字符")]
    public string? LogoUrl { get; set; }

    /// <summary>
    /// 网站Favicon图标URL
    /// </summary>
    [StringLength(200, ErrorMessage = "Favicon URL不能超过200个字符")]
    public string? FaviconUrl { get; set; }

    /// <summary>
    /// 主题颜色（十六进制）
    /// </summary>
    [Required(ErrorMessage = "主题颜色不能为空")]
    [StringLength(50, ErrorMessage = "主题颜色不能超过50个字符")]
    public string ThemeColor { get; set; } = "#1890ff";

    /// <summary>
    /// 是否在首页显示家谱统计数据
    /// </summary>
    public bool ShowStatistics { get; set; } = true;

    /// <summary>
    /// 是否允许未登录用户浏览家谱
    /// </summary>
    public bool AllowGuestBrowse { get; set; } = false;

    /// <summary>
    /// 访问家谱是否需要回答验证问题
    /// </summary>
    public bool RequireVerification { get; set; } = true;

    /// <summary>
    /// 登录失败多少次后锁定账户
    /// </summary>
    [Range(1, 10, ErrorMessage = "登录失败锁定次数必须在1-10之间")]
    public int MaxLoginAttempts { get; set; } = 5;

    /// <summary>
    /// 账户锁定持续时间（分钟）
    /// </summary>
    [Range(5, 1440, ErrorMessage = "账户锁定时间必须在5-1440分钟之间")]
    public int LockoutDuration { get; set; } = 30;

    /// <summary>
    /// 用户会话超时时间（分钟）
    /// </summary>
    [Range(15, 1440, ErrorMessage = "会话超时时间必须在15-1440分钟之间")]
    public int SessionTimeout { get; set; } = 120;

    /// <summary>
    /// 是否启用操作日志记录
    /// </summary>
    public bool EnableOperationLog { get; set; } = true;

    /// <summary>
    /// 日志保留天数
    /// </summary>
    [Range(7, 365, ErrorMessage = "日志保留天数必须在7-365天之间")]
    public int LogRetentionDays { get; set; } = 90;

    /// <summary>
    /// 管理员联系邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 管理员联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话不能超过20个字符")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系地址
    /// </summary>
    [StringLength(500, ErrorMessage = "联系地址不能超过500个字符")]
    public string? ContactAddress { get; set; }

    /// <summary>
    /// 页脚显示的自定义文本
    /// </summary>
    public string? FooterText { get; set; }
}

/// <summary>
/// 更新系统设置请求DTO
/// </summary>
public class UpdateSystemSettingsDto
{
    /// <summary>
    /// 网站名称
    /// </summary>
    [Required(ErrorMessage = "网站名称不能为空")]
    [StringLength(100, ErrorMessage = "网站名称不能超过100个字符")]
    public string SiteName { get; set; } = "家族族谱";

    /// <summary>
    /// 网站描述（用于SEO）
    /// </summary>
    [StringLength(500, ErrorMessage = "网站描述不能超过500个字符")]
    public string? SiteDescription { get; set; }

    /// <summary>
    /// 网站Logo图片URL
    /// </summary>
    [StringLength(200, ErrorMessage = "Logo URL不能超过200个字符")]
    public string? LogoUrl { get; set; }

    /// <summary>
    /// 网站Favicon图标URL
    /// </summary>
    [StringLength(200, ErrorMessage = "Favicon URL不能超过200个字符")]
    public string? FaviconUrl { get; set; }

    /// <summary>
    /// 主题颜色（十六进制）
    /// </summary>
    [Required(ErrorMessage = "主题颜色不能为空")]
    [StringLength(50, ErrorMessage = "主题颜色不能超过50个字符")]
    public string ThemeColor { get; set; } = "#1890ff";

    /// <summary>
    /// 是否在首页显示家谱统计数据
    /// </summary>
    public bool ShowStatistics { get; set; } = true;

    /// <summary>
    /// 是否允许未登录用户浏览家谱
    /// </summary>
    public bool AllowGuestBrowse { get; set; } = false;

    /// <summary>
    /// 访问家谱是否需要回答验证问题
    /// </summary>
    public bool RequireVerification { get; set; } = true;

    /// <summary>
    /// 登录失败多少次后锁定账户
    /// </summary>
    [Range(1, 10, ErrorMessage = "登录失败锁定次数必须在1-10之间")]
    public int MaxLoginAttempts { get; set; } = 5;

    /// <summary>
    /// 账户锁定持续时间（分钟）
    /// </summary>
    [Range(5, 1440, ErrorMessage = "账户锁定时间必须在5-1440分钟之间")]
    public int LockoutDuration { get; set; } = 30;

    /// <summary>
    /// 用户会话超时时间（分钟）
    /// </summary>
    [Range(15, 1440, ErrorMessage = "会话超时时间必须在15-1440分钟之间")]
    public int SessionTimeout { get; set; } = 120;

    /// <summary>
    /// 是否启用操作日志记录
    /// </summary>
    public bool EnableOperationLog { get; set; } = true;

    /// <summary>
    /// 日志保留天数
    /// </summary>
    [Range(7, 365, ErrorMessage = "日志保留天数必须在7-365天之间")]
    public int LogRetentionDays { get; set; } = 90;

    /// <summary>
    /// 管理员联系邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 管理员联系电话
    /// </summary>
    [StringLength(20, ErrorMessage = "联系电话不能超过20个字符")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系地址
    /// </summary>
    [StringLength(500, ErrorMessage = "联系地址不能超过500个字符")]
    public string? ContactAddress { get; set; }

    /// <summary>
    /// 页脚显示的自定义文本
    /// </summary>
    public string? FooterText { get; set; }
}
