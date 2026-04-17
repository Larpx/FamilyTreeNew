using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

/// <summary>
/// 创建菜单时使用的请求模型。
/// 它描述了菜单树中的一个新节点，包括名称、地址、图标和显示规则。
/// </summary>
public class MenuCreateRequestDto
{
    public Guid? ParentId { get; set; }

    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(50, ErrorMessage = "菜单名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "URL不能超过200个字符")]
    public string? Url { get; set; }

    [StringLength(50, ErrorMessage = "图标不能超过50个字符")]
    public string? Icon { get; set; }

    [StringLength(50, ErrorMessage = "权限编码不能超过50个字符")]
    public string? PermissionCode { get; set; }

    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int SortOrder { get; set; } = 0;

    public bool IsEnabled { get; set; } = true;

    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// 更新菜单时使用的请求模型。
/// 适合在后台修改已有菜单的层级、排序和可见性。
/// </summary>
public class MenuUpdateRequestDto
{
    public Guid? ParentId { get; set; }

    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(50, ErrorMessage = "菜单名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "URL不能超过200个字符")]
    public string? Url { get; set; }

    [StringLength(50, ErrorMessage = "图标不能超过50个字符")]
    public string? Icon { get; set; }

    [StringLength(50, ErrorMessage = "权限编码不能超过50个字符")]
    public string? PermissionCode { get; set; }

    [Range(0, 9999, ErrorMessage = "排序值必须在0-9999之间")]
    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsVisible { get; set; }
}

/// <summary>
/// 菜单返回模型。
/// 这个对象通常会构造成树形结构，供前端菜单栏直接渲染使用。
/// </summary>
public class MenuResponseDto
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Url { get; set; }

    public string? Icon { get; set; }

    public string? PermissionCode { get; set; }

    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsVisible { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<MenuResponseDto>? Children { get; set; }
}
