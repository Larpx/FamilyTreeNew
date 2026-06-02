using FamilyTreeNew.Models.DTOs;

namespace FamilyTreeNew.BLL.Services;

/// <summary>
/// 验证服务接口，提供家谱访问验证和令牌管理功能
/// </summary>
public interface IVerificationService
{
    /// <summary>
    /// 验证用户答案
    /// </summary>
    /// <param name="dto">验证答案请求数据</param>
    Task<VerificationResultDto> VerifyAnswerAsync(VerifyAnswerDto dto);

    /// <summary>
    /// 获取家谱验证状态
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    Task<FamilyTreeVerificationStatusDto> GetFamilyTreeVerificationStatusAsync(Guid familyTreeId);

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    /// <param name="familyTreeId">家谱ID</param>
    /// <param name="questionId">验证问题ID</param>
    string GenerateAccessToken(Guid familyTreeId, Guid questionId);

    /// <summary>
    /// 验证访问令牌有效性
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <param name="familyTreeId">家谱ID</param>
    bool ValidateAccessToken(string token, Guid familyTreeId);
}
