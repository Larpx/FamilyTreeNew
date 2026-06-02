using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 验证问题服务接口，提供家谱访问验证问题的增删改查功能
/// </summary>
public interface IVerificationQuestionService
{
    /// <summary>
    /// 获取所有验证问题
    /// </summary>
    Task<List<VerificationQuestionDto>> GetAllAsync();

    /// <summary>
    /// 根据ID获取验证问题
    /// </summary>
    /// <param name="id">验证问题ID</param>
    Task<VerificationQuestionDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 根据家谱ID获取验证问题列表
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    Task<List<VerificationQuestionDto>> GetByFamilyTreeIdAsync(Guid familyTreeId);

    /// <summary>
    /// 创建验证问题
    /// </summary>
    /// <param name="dto">验证问题创建数据</param>
    Task<VerificationQuestionDto> CreateAsync(CreateVerificationQuestionDto dto);

    /// <summary>
    /// 更新验证问题
    /// </summary>
    /// <param name="id">验证问题ID</param>
    /// <param name="dto">验证问题更新数据</param>
    Task<VerificationQuestionDto?> UpdateAsync(Guid id, UpdateVerificationQuestionDto dto);

    /// <summary>
    /// 删除验证问题
    /// </summary>
    /// <param name="id">验证问题ID</param>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 为家谱批量添加验证问题
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <param name="questions">验证问题创建数据列表</param>
    Task<bool> AddQuestionsToFamilyTreeAsync(Guid familyTreeId, List<CreateVerificationQuestionDto> questions);
}
