using FamilyTreeNew.Models.DTOs;
using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public interface IVerificationQuestionService
{
    Task<List<VerificationQuestionDto>> GetAllAsync();
    Task<VerificationQuestionDto?> GetByIdAsync(Guid id);
    Task<List<VerificationQuestionDto>> GetByFamilyTreeIdAsync(Guid familyTreeId);
    Task<VerificationQuestionDto> CreateAsync(CreateVerificationQuestionDto dto);
    Task<VerificationQuestionDto?> UpdateAsync(Guid id, UpdateVerificationQuestionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AddQuestionsToFamilyTreeAsync(Guid familyTreeId, List<CreateVerificationQuestionDto> questions);
}

public class VerificationQuestionService : IVerificationQuestionService
{
    private readonly DAL.Repositories.IVerificationQuestionRepository _repository;
    private readonly DAL.Repositories.IFamilyTreeRepository _familyTreeRepository;

    public VerificationQuestionService(
        DAL.Repositories.IVerificationQuestionRepository repository,
        DAL.Repositories.IFamilyTreeRepository familyTreeRepository)
    {
        _repository = repository;
        _familyTreeRepository = familyTreeRepository;
    }

    public async Task<List<VerificationQuestionDto>> GetAllAsync()
    {
        var questions = await _repository.GetAllWithFamilyTreeAsync();
        return questions.Select(q => MapToDto(q)).ToList();
    }

    public async Task<VerificationQuestionDto?> GetByIdAsync(Guid id)
    {
        var question = await _repository.GetByIdWithFamilyTreeAsync(id);
        return question == null ? null : MapToDto(question);
    }

    public async Task<List<VerificationQuestionDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var questions = await _repository.GetByFamilyTreeIdAsync(familyTreeId);
        return questions.Select(q => MapToDto(q)).ToList();
    }

    public async Task<VerificationQuestionDto> CreateAsync(CreateVerificationQuestionDto dto)
    {
        var familyTree = await _familyTreeRepository.GetByIdAsync(dto.FamilyTreeId);
        if (familyTree == null)
        {
            throw new ArgumentException($"家谱ID {dto.FamilyTreeId} 不存在");
        }

        var entity = new VerificationQuestion
        {
            FamilyTreeId = dto.FamilyTreeId,
            Question = dto.Question,
            AnswerKeyword = dto.AnswerKeyword,
            Order = dto.Order,
            IsEnabled = dto.IsEnabled,
            CreatedAt = DateTime.Now
        };

        await _repository.InsertAsync(entity);
        return MapToDto(entity);
    }

    public async Task<VerificationQuestionDto?> UpdateAsync(Guid id, UpdateVerificationQuestionDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.Question = dto.Question;
        existing.AnswerKeyword = dto.AnswerKeyword;
        existing.Order = dto.Order;
        existing.IsEnabled = dto.IsEnabled;

        await _repository.UpdateAsync(existing);
        return MapToDto(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> AddQuestionsToFamilyTreeAsync(Guid familyTreeId, List<CreateVerificationQuestionDto> questions)
    {
        var familyTree = await _familyTreeRepository.GetByIdAsync(familyTreeId);
        if (familyTree == null)
        {
            return false;
        }

        for (int i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            var entity = new VerificationQuestion
            {
                FamilyTreeId = familyTreeId,
                Question = question.Question,
                AnswerKeyword = question.AnswerKeyword,
                Order = question.Order > 0 ? question.Order : i + 1,
                IsEnabled = question.IsEnabled,
                CreatedAt = DateTime.Now
            };
            await _repository.InsertAsync(entity);
        }

        if (!familyTree.RequireVerification)
        {
            familyTree.RequireVerification = true;
            await _familyTreeRepository.UpdateAsync(familyTree);
        }

        return true;
    }

    private VerificationQuestionDto MapToDto(VerificationQuestion entity)
    {
        return new VerificationQuestionDto
        {
            Id = entity.Id,
            FamilyTreeId = entity.FamilyTreeId,
            Question = entity.Question,
            Order = entity.Order,
            IsEnabled = entity.IsEnabled,
            CreatedAt = entity.CreatedAt,
            FamilyTreeName = entity.FamilyTree?.Name
        };
    }
}
