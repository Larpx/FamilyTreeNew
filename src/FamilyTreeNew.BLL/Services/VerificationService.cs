using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace FamilyTreeNew.BLL.Services;

public class VerificationService : IVerificationService
{
    private readonly DAL.Repositories.IVerificationQuestionRepository _questionRepository;
    private readonly DAL.Repositories.IFamilyTreeRepository _familyTreeRepository;
    private static readonly ConcurrentDictionary<string, TokenEntry> _tokenCache = new();
    private readonly int _tokenExpirationHours;

    /// <summary>
    /// 令牌缓存条目，包含关联的家谱ID和过期时间
    /// </summary>
    private class TokenEntry
    {
        public Guid FamilyTreeId { get; init; }
        public DateTime Expiration { get; init; }
    }

    public VerificationService(
        DAL.Repositories.IVerificationQuestionRepository questionRepository,
        DAL.Repositories.IFamilyTreeRepository familyTreeRepository,
        IConfiguration configuration)
    {
        _questionRepository = questionRepository;
        _familyTreeRepository = familyTreeRepository;
        _tokenExpirationHours = configuration.GetValue("Verification:TokenExpirationHours", 24);
    }

    public async Task<VerificationResultDto> VerifyAnswerAsync(VerifyAnswerDto dto)
    {
        var familyTree = await _familyTreeRepository.GetByIdAsync(dto.FamilyTreeId);
        if (familyTree == null)
        {
            return new VerificationResultDto
            {
                Success = false,
                Message = "家谱不存在"
            };
        }

        if (!familyTree.RequireVerification)
        {
            return new VerificationResultDto
            {
                Success = true,
                Message = "该家谱无需验证",
                AllQuestionsPassed = true
            };
        }

        var question = await _questionRepository.GetByIdWithFamilyTreeAsync(dto.QuestionId);
        if (question == null || question.FamilyTreeId != dto.FamilyTreeId)
        {
            return new VerificationResultDto
            {
                Success = false,
                Message = "验证问题不存在或不属于该家谱"
            };
        }

        if (!question.IsEnabled)
        {
            return new VerificationResultDto
            {
                Success = false,
                Message = "该验证问题已禁用"
            };
        }

        bool isCorrect = CheckAnswer(dto.Answer, question.AnswerKeyword);
        if (!isCorrect)
        {
            return new VerificationResultDto
            {
                Success = false,
                Message = "答案不正确，请重试"
            };
        }

        var allQuestions = await _questionRepository.GetByFamilyTreeIdAsync(dto.FamilyTreeId);
        var enabledQuestions = allQuestions.Where(q => q.IsEnabled).OrderBy(q => q.Order).ToList();

        int currentIndex = enabledQuestions.FindIndex(q => q.Id == dto.QuestionId);
        bool isLastQuestion = currentIndex == enabledQuestions.Count - 1;

        var result = new VerificationResultDto
        {
            Success = true,
            Message = isLastQuestion ? "恭喜！所有验证问题已通过" : "答案正确",
            AllQuestionsPassed = isLastQuestion
        };

        if (isLastQuestion)
        {
            result.AccessToken = GenerateAccessToken(dto.FamilyTreeId, dto.QuestionId);
        }
        else
        {
            result.NextQuestionOrder = enabledQuestions[currentIndex + 1].Order;
        }

        return result;
    }

    public async Task<FamilyTreeVerificationStatusDto> GetFamilyTreeVerificationStatusAsync(Guid familyTreeId)
    {
        var familyTree = await _familyTreeRepository.GetByIdAsync(familyTreeId);
        if (familyTree == null)
        {
            throw new ArgumentException($"家谱ID {familyTreeId} 不存在");
        }

        var questions = await _questionRepository.GetByFamilyTreeIdAsync(familyTreeId);
        var enabledQuestions = questions.Where(q => q.IsEnabled).ToList();

        return new FamilyTreeVerificationStatusDto
        {
            FamilyTreeId = familyTreeId,
            FamilyTreeName = familyTree.Name,
            RequireVerification = familyTree.RequireVerification,
            TotalQuestions = enabledQuestions.Count,
            Questions = enabledQuestions.Select(q => new VerificationQuestionDto
            {
                Id = q.Id,
                FamilyTreeId = q.FamilyTreeId,
                Question = q.Question,
                Order = q.Order,
                IsEnabled = q.IsEnabled,
                CreatedAt = q.CreatedAt
            }).ToList()
        };
    }

    public string GenerateAccessToken(Guid familyTreeId, Guid questionId)
    {
        string rawData = $"{familyTreeId}_{questionId}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";
        string token = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawData)));

        _tokenCache[token] = new TokenEntry
        {
            FamilyTreeId = familyTreeId,
            Expiration = DateTime.UtcNow.AddHours(_tokenExpirationHours)
        };

        CleanExpiredTokens();

        return token;
    }

    public bool ValidateAccessToken(string token, Guid familyTreeId)
    {
        if (!_tokenCache.TryGetValue(token, out TokenEntry? entry))
        {
            return false;
        }

        if (DateTime.UtcNow > entry.Expiration)
        {
            _tokenCache.TryRemove(token, out _);
            return false;
        }

        if (entry.FamilyTreeId != familyTreeId)
        {
            return false;
        }

        return true;
    }

    private bool CheckAnswer(string userAnswer, string keyword)
    {
        if (string.IsNullOrWhiteSpace(userAnswer) || string.IsNullOrWhiteSpace(keyword))
        {
            return false;
        }

        return userAnswer.Trim().Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private void CleanExpiredTokens()
    {
        foreach (var kvp in _tokenCache)
        {
            if (DateTime.UtcNow > kvp.Value.Expiration)
            {
                _tokenCache.TryRemove(kvp.Key, out _);
            }
        }
    }
}
