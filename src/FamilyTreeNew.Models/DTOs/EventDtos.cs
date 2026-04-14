namespace FamilyTreeNew.Models.DTOs;

public class EventTypeCreateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsEnabled { get; set; } = true;
}

public class EventTypeUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int SortOrder { get; set; }
    
    public bool IsEnabled { get; set; }
}

public class EventTypeResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int SortOrder { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

public class EventCreateRequestDto
{
    public Guid EventTypeId { get; set; }
    
    public Guid FamilyTreeId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public Guid? PlaceId { get; set; }
    
    public DateTime? DateSolar { get; set; }
    
    public string? DateLunar { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsPrimary { get; set; } = false;
    
    public string? Remarks { get; set; }
}

public class EventUpdateRequestDto
{
    public Guid EventTypeId { get; set; }
    
    public Guid? PlaceId { get; set; }
    
    public DateTime? DateSolar { get; set; }
    
    public string? DateLunar { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsPrimary { get; set; }
    
    public string? Remarks { get; set; }
}

public class EventResponseDto
{
    public Guid Id { get; set; }
    
    public Guid EventTypeId { get; set; }
    
    public string EventTypeName { get; set; } = string.Empty;
    
    public Guid FamilyTreeId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public string MemberName { get; set; } = string.Empty;
    
    public Guid? PlaceId { get; set; }
    
    public string? PlaceName { get; set; }
    
    public DateTime? DateSolar { get; set; }
    
    public string? DateLunar { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsPrimary { get; set; }
    
    public string? Remarks { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}