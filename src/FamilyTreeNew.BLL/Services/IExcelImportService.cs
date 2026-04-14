using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

public interface IExcelImportService
{
    Task<ExcelImportResultDto> ImportMembersFromExcelAsync(Guid familyTreeId, Stream excelStream);
    byte[] GenerateTemplate();
}
