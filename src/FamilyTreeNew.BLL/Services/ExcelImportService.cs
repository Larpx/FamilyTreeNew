using System.ComponentModel.DataAnnotations;
using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using MiniExcelLibs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// Excel导入服务，使用MiniExcel库处理Excel文件的读取和生成
/// MiniExcel是一个轻量级、低内存占用的Excel处理库，适用于大文件流式读取场景
/// </summary>
public class ExcelImportService : IExcelImportService
{
    private readonly IFamilyMemberRepository _memberRepository;
    private readonly IFamilyTreeRepository _familyTreeRepository;

    public ExcelImportService(IFamilyMemberRepository memberRepository, IFamilyTreeRepository familyTreeRepository)
    {
        _memberRepository = memberRepository;
        _familyTreeRepository = familyTreeRepository;
    }

    /// <summary>
    /// 从Excel流中导入家族成员数据
    /// 使用MiniExcel的Query方法以流式方式读取Excel，避免将整个文件加载到内存中
    /// 读取结果为Dictionary&lt;string, object&gt;行集合，通过列名映射到成员属性
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <param name="excelStream">Excel文件流</param>
    /// <returns>导入结果，包含成功/失败计数和错误详情</returns>
    public async Task<ExcelImportResultDto> ImportMembersFromExcelAsync(Guid familyTreeId, Stream excelStream)
    {
        var result = new ExcelImportResultDto();

        if (!await _familyTreeRepository.ExistsAsync(familyTreeId))
        {
            result.Success = false;
            result.Message = "指定的家谱不存在";
            return result;
        }

        var existingMembers = await _memberRepository.GetByFamilyTreeIdAsync(familyTreeId);
        var memberDict = new Dictionary<string, FamilyMember>();
        foreach (var member in existingMembers)
        {
            var key = $"{member.Surname}{member.FirstName}";
            if (!memberDict.ContainsKey(key))
            {
                memberDict[key] = member;
            }
        }

        var membersToImport = new List<FamilyMember>();

        try
        {
            var rows = excelStream.Query<Dictionary<string, object>>().ToList();

            if (rows.Count == 0)
            {
                result.Success = false;
                result.Message = "Excel文件为空或格式不正确";
                return result;
            }

            result.TotalRows = rows.Count;

            var requiredHeaders = new[] { "姓氏", "名字" };
            var firstRow = rows[0];
            foreach (var header in requiredHeaders)
            {
                if (!firstRow.ContainsKey(header))
                {
                    result.Errors.Add(new ExcelImportErrorDto
                    {
                        RowNumber = 1,
                        FieldName = header,
                        ErrorMessage = $"缺少必需的列: {header}"
                    });
                }
            }

            if (result.Errors.Count > 0)
            {
                result.Success = false;
                result.Message = "Excel文件格式不正确";
                return result;
            }

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var excelRowNumber = i + 2;

                try
                {
                    var surname = GetCellValue(row, "姓氏");
                    var firstName = GetCellValue(row, "名字");

                    if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(firstName))
                    {
                        result.Errors.Add(new ExcelImportErrorDto
                        {
                            RowNumber = excelRowNumber,
                            FieldName = "姓名",
                            ErrorMessage = "姓氏和名字不能为空",
                            RowData = $"姓氏: {surname}, 名字: {firstName}"
                        });
                        continue;
                    }

                    var parentName = GetCellValue(row, "父成员姓名");
                    Guid? parentId = null;

                    if (!string.IsNullOrWhiteSpace(parentName))
                    {
                        if (memberDict.TryGetValue(parentName, out var parentMember))
                        {
                            parentId = parentMember.Id;
                        }
                        else
                        {
                            result.Errors.Add(new ExcelImportErrorDto
                            {
                                RowNumber = excelRowNumber,
                                FieldName = "父成员姓名",
                                ErrorMessage = $"未找到父成员: {parentName}",
                                RowData = $"父成员姓名: {parentName}"
                            });
                            continue;
                        }
                    }

                    var generation = await CalculateGenerationAsync(parentId, membersToImport, memberDict);

                    var member = new FamilyMember
                    {
                        Id = Guid.NewGuid(),
                        FamilyTreeId = familyTreeId,
                        ParentId = parentId,
                        Generation = generation,
                        Surname = surname,
                        FirstName = firstName,
                        Alias = GetCellValue(row, "字号别称"),
                        Ranking = GetCellValue(row, "排行"),
                        GenerationName = GetCellValue(row, "字辈"),
                        BirthDateLunar = GetCellValue(row, "生辰农历"),
                        Residence = GetCellValue(row, "居住地"),
                        Occupation = GetCellValue(row, "职业"),
                        PersonalInfo = GetCellValue(row, "个人信息"),
                        Note = GetCellValue(row, "小注"),
                        DeathDateLunar = GetCellValue(row, "卒亡农历"),
                        Remarks = GetCellValue(row, "备注"),
                        CreatedAt = DateTime.Now
                    };

                    var birthDateStr = GetCellValue(row, "生辰公历");
                    if (!string.IsNullOrWhiteSpace(birthDateStr) && DateTime.TryParse(birthDateStr, out var birthDate))
                    {
                        member.BirthDateSolar = birthDate;
                    }

                    var deathDateStr = GetCellValue(row, "卒亡公历");
                    if (!string.IsNullOrWhiteSpace(deathDateStr) && DateTime.TryParse(deathDateStr, out var deathDate))
                    {
                        member.DeathDateSolar = deathDate;
                        member.IsDeceased = true;
                    }

                    var isDeceasedStr = GetCellValue(row, "是否卒亡");
                    if (!string.IsNullOrWhiteSpace(isDeceasedStr))
                    {
                        member.IsDeceased = isDeceasedStr.Equals("是", StringComparison.OrdinalIgnoreCase) ||
                                           isDeceasedStr.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                           isDeceasedStr.Equals("1");
                    }

                    membersToImport.Add(member);
                    var memberKey = $"{member.Surname}{member.FirstName}";
                    if (!memberDict.ContainsKey(memberKey))
                    {
                        memberDict[memberKey] = member;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ExcelImportErrorDto
                    {
                        RowNumber = excelRowNumber,
                        FieldName = "处理错误",
                        ErrorMessage = ex.Message
                    });
                }
            }

            if (membersToImport.Count > 0)
            {
                await _memberRepository.BulkInsertAsync(membersToImport);
                result.SuccessCount = membersToImport.Count;
            }

            result.FailCount = result.Errors.Count;
            result.Success = result.Errors.Count == 0;
            result.Message = result.Success
                ? $"成功导入 {result.SuccessCount} 条记录"
                : $"导入完成，成功 {result.SuccessCount} 条，失败 {result.FailCount} 条";

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"导入过程中发生错误: {ex.Message}";
            return result;
        }
    }

    public byte[] GenerateTemplate()
    {
        var rows = new List<object>
        {
            new
            {
                姓氏 = "张", 名字 = "三", 父成员姓名 = "", 字号别称 = "字明远", 排行 = "长子", 字辈 = "永",
                生辰公历 = "1950-01-01", 生辰农历 = "腊月初八", 居住地 = "北京市", 职业 = "教师",
                个人信息 = "家族长辈", 小注 = "", 是否卒亡 = "否", 卒亡公历 = "", 卒亡农历 = "", 备注 = ""
            },
            new
            {
                姓氏 = "张", 名字 = "四", 父成员姓名 = "张三", 字号别称 = "", 排行 = "次子", 字辈 = "志",
                生辰公历 = "1975-06-15", 生辰农历 = "", 居住地 = "上海市", 职业 = "工程师",
                个人信息 = "", 小注 = "", 是否卒亡 = "否", 卒亡公历 = "", 卒亡农历 = "", 备注 = ""
            }
        };

        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return stream.ToArray();
    }

    private static string? GetCellValue(Dictionary<string, object> row, string headerName)
    {
        if (!row.TryGetValue(headerName, out var value) || value == null)
        {
            return null;
        }

        return value.ToString()?.Trim();
    }

    private async Task<int> CalculateGenerationAsync(Guid? parentId, List<FamilyMember> pendingMembers, Dictionary<string, FamilyMember> existingMembers)
    {
        if (!parentId.HasValue)
        {
            return 1;
        }

        var parent = pendingMembers.FirstOrDefault(m => m.Id == parentId.Value);
        if (parent != null)
        {
            return (parent.Generation ?? 0) + 1;
        }

        var generation = await _memberRepository.GetGenerationByParentIdAsync(parentId);
        return generation ?? 1;
    }
}
