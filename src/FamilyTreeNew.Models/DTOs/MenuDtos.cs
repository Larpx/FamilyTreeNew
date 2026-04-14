namespace FamilyTreeNew.Models.DTOs;

public class MenuCreateRequestDto
{
    public Guid? ParentId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Url { get; set; }
    
    public string? Icon { get; set; }
    
    public string? PermissionCode { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsEnabled { get; set; } = true;
    
    public bool IsVisible { get; set; } = true;
}

public class MenuUpdateRequestDto
{
    public Guid? ParentId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Url { get; set; }
    
    public string? Icon { get; set; }
    
    public string? PermissionCode { get; set; }
    
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