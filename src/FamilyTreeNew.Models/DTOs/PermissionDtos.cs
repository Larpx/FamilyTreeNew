using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建权限时使用的请求模型。
/// 用来描述一个权限节点的编码、名称、类型和父子关系。
/// </summary>
public class PermissionCreateRequestDto
{
    [Required(ErrorMessage = "权限编码不能为空")]
    [StringLength(50, ErrorMessage = "权限编码不能超过50个字符")]
    [RegularExpression(@"^[A-Za-z_][A-Za-z0-9_.]*$", ErrorMessage = "权限编码只能包含字母、数字、下划线和点，且必须以字母或下划线开头")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "权限名称不能为空")]
    [StringLength(50, ErrorMessage = "权限名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "权限类型不能为空")]
    [StringLength(20, ErrorMessage = "权限类型不能超过20个字符")]
    public string Type { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "URL不能超过200个字符")]
    public string? Url { get; set; }

    [StringLength(10, ErrorMessage = "HTTP方法不能超过10个字符")]
    public string? Method { get; set; }

    public Guid? ParentId { get; set; }

    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int SortOrder { get; set; } = 0;

    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新权限时使用的请求模型。
/// 适用于修改权限名称、类型、地址和启用状态等信息。
/// </summary>
public class PermissionUpdateRequestDto
{
    [Required(ErrorMessage = "权限名称不能为空")]
    [StringLength(50, ErrorMessage = "权限名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "权限类型不能为空")]
    [StringLength(20, ErrorMessage = "权限类型不能超过20个字符")]
    public string Type { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "URL不能超过200个字符")]
    public string? Url { get; set; }

    [StringLength(10, ErrorMessage = "HTTP方法不能超过10个字符")]
    public string? Method { get; set; }

    public Guid? ParentId { get; set; }

    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }
}

/// <summary>
/// 权限返回模型。
/// 后台可以借助它展示权限树、权限列表以及节点状态。
/// </summary>
public class PermissionResponseDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Url { get; set; }

    public string? Method { get; set; }

    public Guid? ParentId { get; set; }

    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PermissionResponseDto>? Children { get; set; }
}
