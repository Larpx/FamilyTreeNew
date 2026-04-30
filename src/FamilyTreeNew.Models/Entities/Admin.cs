using SqlSugar;

namespace FamilyTreeNew.Models.Entities;

/// <summary>
/// 管理员实体类
/// 存储系统管理员账户信息，包括登录凭证和基本资料等
/// </summary>
[SugarTable("Admins")]
public class Admin
{
    /// <summary>
    /// 管理员唯一标识符
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "管理员ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 登录用户名，最大长度50字符
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "用户名")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 登录密码，使用加密算法存储
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = false, ColumnDescription = "密码（加密存储）")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 密码加密盐值，用于增强密码安全性
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "密码盐值")]
    public string? PasswordSalt { get; set; }

    /// <summary>
    /// 管理员真实姓名
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "真实姓名")]
    public string? RealName { get; set; }

    /// <summary>
    /// 联系邮箱地址
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 账户创建时间
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "创建日期")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最后一次登录系统的时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "最后登录时间")]
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 账户是否启用，true为启用，false为禁用
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

}
