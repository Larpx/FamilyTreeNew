namespace FamilyTreeNew.BLL.Services;

public interface IGedcomService
{
    Task<string> ExportToGedcomAsync(Guid familyTreeId);

    Task<(bool Success, string Message)> ImportFromGedcomAsync(Guid familyTreeId, string gedcomContent);
}