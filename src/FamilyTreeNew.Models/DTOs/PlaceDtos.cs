namespace FamilyTreeNew.Models.DTOs;

public class PlaceCreateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Address { get; set; }
    
    public string? Province { get; set; }
    
    public string? City { get; set; }
    
    public string? District { get; set; }
    
    public decimal? Latitude { get; set; }
    
    public decimal? Longitude { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsEnabled { get; set; } = true;
}

public class PlaceUpdateRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Address { get; set; }
    
    public string? Province { get; set; }
    
    public string? City { get; set; }
    
    public string? District { get; set; }
    
    public decimal? Latitude { get; set; }
    
    public decimal? Longitude { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsEnabled { get; set; }
}

public class PlaceResponseDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Address { get; set; }
    
    public string? Province { get; set; }
    
    public string? City { get; set; }
    
    public string? District { get; set; }
    
    public decimal? Latitude { get; set; }
    
    public decimal? Longitude { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}