using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建角色时使用的请求模型。
/// 它定义了角色名称、编码、说明和启用状态。
/// </summary>
public class RoleCreateRequestDto
{
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, ErrorMessage = "角色名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "角色描述不能超过200个字符")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "角色编码不能为空")]
    [StringLength(50, ErrorMessage = "角色编码不能超过50个字符")]
    [RegularExpression(@"^[A-Za-z_][A-Za-z0-9_]*$", ErrorMessage = "角色编码只能包含字母、数字和下划线，且必须以字母或下划线开头")]
    public string Code { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新角色时使用的请求模型。
/// 用于修改角色名称、描述和启用状态。
/// </summary>
public class RoleUpdateRequestDto
{
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, ErrorMessage = "角色名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "角色描述不能超过200个字符")]
    public string? Description { get; set; }

    public bool IsEnabled { get; set; }
}

/// <summary>
/// 角色返回模型。
/// 后台页面会使用它显示角色及其关联的权限信息。
/// </summary>
public class RoleResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Code { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<PermissionResponseDto>? Permissions { get; set; }
}

/// <summary>
/// 给角色分配权限时使用的请求模型。
/// 只需要传入要绑定的权限 ID 列表即可。
/// </summary>
public class RolePermissionAssignRequestDto
{
    [Required(ErrorMessage = "权限ID列表不能为空")]
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}
