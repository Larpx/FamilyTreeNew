using System.ComponentModel.DataAnnotations;

namespace FamilyTreeNew.Models.DTOs;

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
