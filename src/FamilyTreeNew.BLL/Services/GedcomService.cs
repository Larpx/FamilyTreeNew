using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.Entities;
using FamilyTreeNew.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// GEDCOM格式导入导出服务
/// 支持将家谱数据导出为GEDCOM 5.5.1格式，以及从GEDCOM文件导入成员数据
/// </summary>
public class GedcomService : IGedcomService
{
    private readonly IFamilyTreeRepository _familyTreeRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;
    private readonly IFamilyMemberService _familyMemberService;
    private readonly ILogger<GedcomService> _logger;

    public GedcomService(IFamilyTreeRepository familyTreeRepository,
        IFamilyMemberRepository familyMemberRepository,
        IFamilyMemberService familyMemberService,
        ILogger<GedcomService> logger)
    {
        _familyTreeRepository = familyTreeRepository;
        _familyMemberRepository = familyMemberRepository;
        _familyMemberService = familyMemberService;
        _logger = logger;
    }

    public async Task<string> ExportToGedcomAsync(Guid familyTreeId)
    {
        var familyTree = await _familyTreeRepository.GetByIdAsync(familyTreeId);
        if (familyTree == null)
        {
            throw new Exception("家谱不存在");
        }

        var members = await _familyMemberRepository.GetByFamilyTreeIdAsync(familyTreeId);

        var gedcom = new StringBuilder();
        gedcom.AppendLine("0 HEAD");
        gedcom.AppendLine("1 SOUR FamilyTreeNew");
        gedcom.AppendLine("1 NAME " + EscapeGedcomString(familyTree.Name));
        gedcom.AppendLine("1 GEDC");
        gedcom.AppendLine("2 VERS 5.5.1");
        gedcom.AppendLine("2 FORM LINEAGE-LINKED");
        gedcom.AppendLine("1 CHAR UTF-8");
        gedcom.AppendLine("1 DATE " + DateTime.UtcNow.ToString("yyyy-MM-dd"));

        foreach (var member in members)
        {
            gedcom.AppendLine();
            gedcom.AppendLine($"0 @I{member.Id.ToString().Replace("-", "")}@ INDI");
            gedcom.AppendLine($"1 NAME {EscapeGedcomString(member.Surname)} /{EscapeGedcomString(member.FirstName)}/");

            if (!string.IsNullOrEmpty(member.Alias))
            {
                gedcom.AppendLine($"1 NICK {EscapeGedcomString(member.Alias)}");
            }

            if (!string.IsNullOrEmpty(member.GenerationName))
            {
                gedcom.AppendLine($"1 TITL {EscapeGedcomString(member.GenerationName)}");
            }

            if (member.BirthDateSolar.HasValue)
            {
                gedcom.AppendLine($"1 BIRT");
                gedcom.AppendLine($"2 DATE {member.BirthDateSolar.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}");
            }

            if (!string.IsNullOrEmpty(member.BirthDateLunar))
            {
                gedcom.AppendLine($"2 NOTE 农历: {EscapeGedcomString(member.BirthDateLunar)}");
            }

            if (member.IsDeceased)
            {
                gedcom.AppendLine($"1 DEAT");
                if (member.DeathDateSolar.HasValue)
                {
                    gedcom.AppendLine($"2 DATE {member.DeathDateSolar.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}");
                }
                if (!string.IsNullOrEmpty(member.DeathDateLunar))
                {
                    gedcom.AppendLine($"2 NOTE 农历: {EscapeGedcomString(member.DeathDateLunar)}");
                }
            }

            if (!string.IsNullOrEmpty(member.Residence))
            {
                gedcom.AppendLine($"1 RESI");
                gedcom.AppendLine($"2 PLAC {EscapeGedcomString(member.Residence)}");
            }

            if (!string.IsNullOrEmpty(member.Occupation))
            {
                gedcom.AppendLine($"1 OCCU {EscapeGedcomString(member.Occupation)}");
            }

            if (!string.IsNullOrEmpty(member.PersonalInfo))
            {
                gedcom.AppendLine($"1 NOTE {EscapeGedcomString(member.PersonalInfo)}");
            }

            if (member.ParentId.HasValue)
            {
                gedcom.AppendLine($"1 FAMC @F{member.ParentId.Value.ToString().Replace("-", "")}@");
            }
        }

        gedcom.AppendLine();
        gedcom.AppendLine("0 TRLR");

        _logger.LogInformation("导出GEDCOM文件，家谱ID: {FamilyTreeId}，成员数: {MemberCount}", familyTreeId, members.Count);
        return gedcom.ToString();
    }

    public async Task<(bool Success, string Message)> ImportFromGedcomAsync(Guid familyTreeId, string gedcomContent)
    {
        try
        {
            var lines = gedcomContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var memberMap = new Dictionary<string, Guid>();
            Guid? currentMemberId = null;
            FamilyMemberCreateDto? currentMember = null;
            string currentTag = string.Empty;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                var parts = trimmedLine.Split(new[] { ' ' }, 3);
                if (parts.Length < 2) continue;

                if (!int.TryParse(parts[0], out int level)) continue;
                string tag = parts[1];
                string value = parts.Length > 2 ? parts[2] : string.Empty;

                if (level == 0)
                {
                    if (currentMember != null && currentMemberId.HasValue)
                    {
                        await CreateMember(currentMember, memberMap);
                    }

                    if (value == "INDI" && tag.StartsWith("@I") && tag.EndsWith("@"))
                    {
                        currentMemberId = Guid.NewGuid();
                        memberMap[tag] = currentMemberId.Value;
                        currentMember = new FamilyMemberCreateDto();
                        currentMember.FamilyTreeId = familyTreeId;
                    }
                    else
                    {
                        currentMember = null;
                        currentMemberId = null;
                    }
                }
                else if (level == 1 && currentMember != null)
                {
                    currentTag = tag;
                    switch (tag)
                    {
                        case "NAME":
                            var nameParts = value.Split('/');
                            if (nameParts.Length >= 2)
                            {
                                currentMember.Surname = UnescapeGedcomString(nameParts[0].Trim());
                                currentMember.FirstName = UnescapeGedcomString(nameParts[1].Trim());
                            }
                            break;
                        case "NICK":
                            currentMember.Alias = UnescapeGedcomString(value);
                            break;
                        case "TITL":
                            currentMember.GenerationName = UnescapeGedcomString(value);
                            break;
                        case "SEX":
                            currentMember.Gender = value;
                            break;
                        case "FAMC":
                            if (memberMap.TryGetValue(value, out Guid parentId))
                            {
                                currentMember.ParentId = parentId;
                            }
                            break;
                    }
                }
                else if (level == 2 && currentMember != null)
                {
                    if (currentTag == "BIRT" && tag == "DATE")
                    {
                        currentMember.BirthDateSolar = ParseGedcomDate(value);
                    }
                    else if (currentTag == "DEAT" && tag == "DATE")
                    {
                        currentMember.DeathDateSolar = ParseGedcomDate(value);
                        currentMember.IsDeceased = true;
                    }
                    else if (currentTag == "RESI" && tag == "PLAC")
                    {
                        currentMember.Residence = UnescapeGedcomString(value);
                    }
                    else if (tag == "OCCU")
                    {
                        currentMember.Occupation = UnescapeGedcomString(value);
                    }
                    else if (tag == "NOTE")
                    {
                        currentMember.PersonalInfo = UnescapeGedcomString(value);
                    }
                }
            }

            if (currentMember != null && currentMemberId.HasValue)
            {
                await CreateMember(currentMember, memberMap);
            }

            _logger.LogInformation("导入GEDCOM文件，家谱ID: {FamilyTreeId}，导入成员数: {MemberCount}", familyTreeId, memberMap.Count);
            return (true, $"成功导入 {memberMap.Count} 个成员");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导入GEDCOM文件失败，家谱ID: {FamilyTreeId}", familyTreeId);
            return (false, "导入失败，请检查文件格式");
        }
    }

    private async Task CreateMember(FamilyMemberCreateDto member, Dictionary<string, Guid> memberMap)
    {
        if (member.ParentId.HasValue && !memberMap.ContainsValue(member.ParentId.Value))
        {
            member.ParentId = null;
        }
        await _familyMemberService.CreateAsync(member);
    }

    private string EscapeGedcomString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Replace("@", "@@");
    }

    private string UnescapeGedcomString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Replace("@@", "@");
    }

    private DateTime? ParseGedcomDate(string dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;

        var formats = new[] { "dd MMM yyyy", "MMM yyyy", "yyyy" };
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
        }
        return null;
    }
}
