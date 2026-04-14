namespace FamilyTreeNew.Models.DTOs;

public class PermissionCreateRequestDto
{
    public string Code { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public string? Url { get; set; }
    
    public string? Method { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsEnabled { get; set; } = true;
}

public class PermissionUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public string? Url { get; set; }
    
    public string? Method { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public int SortOrder { get; set; }
    
    public bool IsEnabled { get; set; }
}

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