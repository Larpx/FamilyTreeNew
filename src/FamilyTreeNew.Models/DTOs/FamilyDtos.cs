namespace FamilyTreeNew.Models.DTOs;

public class SpousalRelationCreateRequestDto
{
    public Guid FamilyTreeId { get; set; }
    
    public Guid HusbandId { get; set; }
    
    public Guid WifeId { get; set; }
    
    public DateTime? MarriageDateSolar { get; set; }
    
    public string? MarriageDateLunar { get; set; }
    
    public string? Status { get; set; }
    
    public bool IsDivorced { get; set; } = false;
    
    public DateTime? DivorceDateSolar { get; set; }
    
    public string? DivorceDateLunar { get; set; }
    
    public string? Remarks { get; set; }
}

public class SpousalRelationUpdateRequestDto
{
    public DateTime? MarriageDateSolar { get; set; }
    
    public string? MarriageDateLunar { get; set; }
    
    public string? Status { get; set; }
    
    public bool IsDivorced { get; set; }
    
    public DateTime? DivorceDateSolar { get; set; }
    
    public string? DivorceDateLunar { get; set; }
    
    public string? Remarks { get; set; }
}

public class SpousalRelationResponseDto
{
    public Guid Id { get; set; }
    
    public Guid FamilyTreeId { get; set; }
    
    public Guid HusbandId { get; set; }
    
    public string HusbandName { get; set; } = string.Empty;
    
    public Guid WifeId { get; set; }
    
    public string WifeName { get; set; } = string.Empty;
    
    public DateTime? MarriageDateSolar { get; set; }
    
    public string? MarriageDateLunar { get; set; }
    
    public string? Status { get; set; }
    
    public bool IsDivorced { get; set; }
    
    public DateTime? DivorceDateSolar { get; set; }
    
    public string? DivorceDateLunar { get; set; }
    
    public string? Remarks { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}