using FamilyTreeNew.DAL.Repositories;
using FamilyTreeNew.Models.DTOs;
using System.Globalization;
using System.Text;

namespace FamilyTreeNew.BLL.Services;

public class GedcomService : IGedcomService
{
    private sealed class PendingMember
    {
        public required string ExternalId { get; init; }
        public required FamilyMemberCreateDto Member { get; init; }
        public string? ParentExternalId { get; set; }
    }

    private readonly IFamilyTreeRepository _familyTreeRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;
    private readonly IFamilyMemberService _familyMemberService;

    public GedcomService(IFamilyTreeRepository familyTreeRepository,
        IFamilyMemberRepository familyMemberRepository,
        IFamilyMemberService familyMemberService)
    {
        _familyTreeRepository = familyTreeRepository;
        _familyMemberRepository = familyMemberRepository;
        _familyMemberService = familyMemberService;
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
                gedcom.AppendLine("1 BIRT");
                gedcom.AppendLine($"2 DATE {member.BirthDateSolar.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}");
            }

            if (!string.IsNullOrEmpty(member.BirthDateLunar))
            {
                gedcom.AppendLine($"2 NOTE 农历: {EscapeGedcomString(member.BirthDateLunar)}");
            }

            if (member.IsDeceased)
            {
                gedcom.AppendLine("1 DEAT");
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
                gedcom.AppendLine("1 RESI");
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
                gedcom.AppendLine($"1 FAMC @I{member.ParentId.Value.ToString().Replace("-", "")}@");
            }
        }

        gedcom.AppendLine();
        gedcom.AppendLine("0 TRLR");

        return gedcom.ToString();
    }

    public async Task<(bool Success, string Message)> ImportFromGedcomAsync(Guid familyTreeId, string gedcomContent)
    {
        try
        {
            var familyTree = await _familyTreeRepository.GetByIdAsync(familyTreeId);
            if (familyTree == null)
            {
                return (false, "家谱不存在");
            }

            if (string.IsNullOrWhiteSpace(gedcomContent))
            {
                return (false, "GEDCOM内容不能为空");
            }

            var lines = gedcomContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var pendingMembers = ParseMembers(lines, familyTreeId);

            if (pendingMembers.Count == 0)
            {
                return (false, "未解析到有效的成员数据");
            }

            var importedCount = await ImportMembersAsync(pendingMembers);
            return (true, $"成功导入 {importedCount} 个成员");
        }
        catch (Exception ex)
        {
            return (false, $"导入失败: {ex.Message}");
        }
    }

    private List<PendingMember> ParseMembers(string[] lines, Guid familyTreeId)
    {
        var result = new List<PendingMember>();
        PendingMember? current = null;
        string currentTag = string.Empty;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            var parts = trimmedLine.Split(' ', 3);
            if (parts.Length < 2 || !int.TryParse(parts[0], out var level))
            {
                continue;
            }

            var tag = parts[1];
            var value = parts.Length > 2 ? parts[2] : string.Empty;

            if (level == 0)
            {
                if (current != null)
                {
                    result.Add(current);
                }

                if (parts.Length == 3 && parts[1].StartsWith("@I", StringComparison.OrdinalIgnoreCase) && parts[1].EndsWith("@") && parts[2] == "INDI")
                {
                    current = new PendingMember
                    {
                        ExternalId = parts[1],
                        Member = new FamilyMemberCreateDto
                        {
                            FamilyTreeId = familyTreeId
                        }
                    };
                    currentTag = string.Empty;
                }
                else
                {
                    current = null;
                }

                continue;
            }

            if (current == null)
            {
                continue;
            }

            if (level == 1)
            {
                currentTag = tag;
                switch (tag)
                {
                    case "NAME":
                        var nameParts = value.Split('/');
                        if (nameParts.Length >= 2)
                        {
                            current.Member.Surname = UnescapeGedcomString(nameParts[0].Trim());
                            current.Member.FirstName = UnescapeGedcomString(nameParts[1].Trim());
                        }
                        break;
                    case "NICK":
                        current.Member.Alias = UnescapeGedcomString(value);
                        break;
                    case "TITL":
                        current.Member.GenerationName = UnescapeGedcomString(value);
                        break;
                    case "SEX":
                        current.Member.Gender = value;
                        break;
                    case "FAMC":
                        current.ParentExternalId = value;
                        break;
                    case "OCCU":
                        current.Member.Occupation = UnescapeGedcomString(value);
                        break;
                    case "NOTE":
                        current.Member.PersonalInfo = UnescapeGedcomString(value);
                        break;
                }
                continue;
            }

            if (level == 2)
            {
                if (currentTag == "BIRT" && tag == "DATE")
                {
                    current.Member.BirthDateSolar = ParseGedcomDate(value);
                }
                else if (currentTag == "BIRT" && tag == "NOTE")
                {
                    current.Member.BirthDateLunar = ParseLunarNote(value);
                }
                else if (currentTag == "DEAT" && tag == "DATE")
                {
                    current.Member.DeathDateSolar = ParseGedcomDate(value);
                    current.Member.IsDeceased = true;
                }
                else if (currentTag == "DEAT" && tag == "NOTE")
                {
                    current.Member.DeathDateLunar = ParseLunarNote(value);
                    current.Member.IsDeceased = true;
                }
                else if (currentTag == "RESI" && tag == "PLAC")
                {
                    current.Member.Residence = UnescapeGedcomString(value);
                }
                else if (tag == "NOTE")
                {
                    current.Member.PersonalInfo = UnescapeGedcomString(value);
                }
            }
        }

        if (current != null)
        {
            result.Add(current);
        }

        return result.Where(m => !string.IsNullOrWhiteSpace(m.Member.Surname) || !string.IsNullOrWhiteSpace(m.Member.FirstName)).ToList();
    }

    private async Task<int> ImportMembersAsync(List<PendingMember> pendingMembers)
    {
        var importedCount = 0;
        var createdIdMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        while (pendingMembers.Count > 0)
        {
            var progressed = false;

            for (var i = pendingMembers.Count - 1; i >= 0; i--)
            {
                var pending = pendingMembers[i];

                if (!string.IsNullOrWhiteSpace(pending.ParentExternalId))
                {
                    if (!createdIdMap.TryGetValue(pending.ParentExternalId, out var parentId))
                    {
                        continue;
                    }

                    pending.Member.ParentId = parentId;
                }
                else
                {
                    pending.Member.ParentId = null;
                }

                var created = await _familyMemberService.CreateAsync(pending.Member);
                createdIdMap[pending.ExternalId] = created.Id;
                pendingMembers.RemoveAt(i);
                importedCount++;
                progressed = true;
            }

            if (!progressed)
            {
                for (var i = pendingMembers.Count - 1; i >= 0; i--)
                {
                    var pending = pendingMembers[i];
                    pending.Member.ParentId = null;
                    var created = await _familyMemberService.CreateAsync(pending.Member);
                    createdIdMap[pending.ExternalId] = created.Id;
                    pendingMembers.RemoveAt(i);
                    importedCount++;
                }
            }
        }

        return importedCount;
    }

    private static string EscapeGedcomString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Replace("@", "@@");
    }

    private static string UnescapeGedcomString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Replace("@@", "@");
    }

    private static string? ParseLunarNote(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var note = UnescapeGedcomString(value).Trim();
        const string prefix = "农历:";
        return note.StartsWith(prefix, StringComparison.Ordinal) ? note[prefix.Length..].Trim() : note;
    }

    private static DateTime? ParseGedcomDate(string dateStr)
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
