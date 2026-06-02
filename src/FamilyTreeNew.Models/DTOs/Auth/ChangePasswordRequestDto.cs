using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs.Auth;

/// <summary>
/// 修改密码请求DTO
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// 原密码
    /// </summary>
    [Required(ErrorMessage = "原密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "新密码长度必须在8-100个字符之间")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认密码（必须与新密码一致）
    /// </summary>
    [Required(ErrorMessage = "确认密码不能为空")]
    [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
