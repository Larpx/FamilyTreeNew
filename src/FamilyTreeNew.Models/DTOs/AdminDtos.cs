using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

public class AdminDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? RealName { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public bool IsEnabled { get; set; }
}

public class CreateAdminDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "真实姓名不能超过100个字符")]
    public string? RealName { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }
}

public class UpdateAdminDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "真实姓名不能超过100个字符")]
    public string? RealName { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }

    public bool IsEnabled { get; set; } = true;
}
