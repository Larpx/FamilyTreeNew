namespace FamilyTreeNew.Models.DTOs;

public class SourceCreateRequestDto
{
    public string Title { get; set; } = string.Empty;
    
    public string? Author { get; set; }
    
    public string? Publisher { get; set; }
    
    public int? Year { get; set; }
    
    public string? Url { get; set; }
    
    public string? Type { get; set; }
    
    public string? Description { get; set; }
    
    public string? Citation { get; set; }
    
    public bool IsEnabled { get; set; } = true;
}

public class SourceUpdateRequestDto
{
    public string Title { get; set; } = string.Empty;
    
    public string? Author { get; set; }
    
    public string? Publisher { get; set; }
    
    public int? Year { get; set; }
    
    public string? Url { get; set; }
    
    public string? Type { get; set; }
    
    public string? Description { get; set; }
    
    public string? Citation { get; set; }
    
    public bool IsEnabled { get; set; }
}

public class SourceResponseDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Author { get; set; }
    
    public string? Publisher { get; set; }
    
    public int? Year { get; set; }
    
    public string? Url { get; set; }
    
    public string? Type { get; set; }
    
    public string? Description { get; set; }
    
    public string? Citation { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}

public class SourceCitationCreateRequestDto
{
    public Guid SourceId { get; set; }
    
    public string TargetType { get; set; } = string.Empty;
    
    public Guid TargetId { get; set; }
    
    public string? Note { get; set; }
}

public class SourceCitationResponseDto
{
    public Guid Id { get; set; }
    
    public Guid SourceId { get; set; }
    
    public string SourceTitle { get; set; } = string.Empty;
    
    public string TargetType { get; set; } = string.Empty;
    
    public Guid TargetId { get; set; }
    
    public string? Note { get; set; }
    
    public DateTime CreatedAt { get; set; }
}