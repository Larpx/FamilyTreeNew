using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public interface IVerificationService
{
    Task<VerificationResultDto> VerifyAnswerAsync(VerifyAnswerDto dto);
    Task<FamilyTreeVerificationStatusDto> GetFamilyTreeVerificationStatusAsync(Guid familyTreeId);
    string GenerateAccessToken(Guid familyTreeId, Guid questionId);
    bool ValidateAccessToken(string token, Guid familyTreeId);
}

public class VerificationService : IVerificationService
{
    private readonly DAL.Repositories.IVerificationQuestionRepository _questionRepository;
    private readonly DAL.Repositories.IFamilyTreeRepository _familyTreeRepository;
    private static readonly ConcurrentDictionary<string, DateTime> _tokenCache = new();
    private const int TokenExpirationHours = 24;

    public VerificationService(
        DAL.Repositories.IVerificationQuestionRepository questionRepository,
        DAL.Repositories.IFamilyTreeRepository familyTreeRepository)
    {
        _questionRepository = questionRepository;
        _familyTreeRepository = familyTreeRepository;
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

        _tokenCache[token] = DateTime.UtcNow.AddHours(TokenExpirationHours);

        CleanExpiredTokens();

        return token;
    }

    public bool ValidateAccessToken(string token, Guid familyTreeId)
    {
        if (!_tokenCache.TryGetValue(token, out DateTime expiration))
        {
            return false;
        }

        if (DateTime.UtcNow > expiration)
        {
            _tokenCache.TryRemove(token, out _);
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
            if (DateTime.UtcNow > kvp.Value)
            {
                _tokenCache.TryRemove(kvp.Key, out _);
            }
        }
    }
}
