namespace FamilyTreeNew.Models.DTOs;

public class RoleCreateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public bool IsEnabled { get; set; } = true;
}

public class RoleUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsEnabled { get; set; }
}

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

public class RolePermissionAssignRequestDto
{
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}