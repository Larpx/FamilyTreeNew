using FamilyTreeNew.Models.Entities;

namespace FamilyTreeNew.BLL.Services;

public interface IFamilyService
{
    Task<List<Family>> GetAllFamiliesAsync();
    Task<Family?> GetFamilyByIdAsync(int id);
    Task<int> CreateFamilyAsync(Family family);
    Task<int> UpdateFamilyAsync(Family family);
    Task<int> DeleteFamilyAsync(int id);
}

public class FamilyService : IFamilyService
{
    private readonly DAL.Repositories.IFamilyRepository _familyRepository;

    public FamilyService(DAL.Repositories.IFamilyRepository familyRepository)
    {
        _familyRepository = familyRepository;
    }

    public async Task<List<Family>> GetAllFamiliesAsync()
    {
        return await _familyRepository.GetFamiliesWithMemberCountAsync();
    }

    public async Task<Family?> GetFamilyByIdAsync(int id)
    {
        return await _familyRepository.GetByIdAsync(id);
    }

    public async Task<int> CreateFamilyAsync(Family family)
    {
        family.CreatedAt = DateTime.Now;
        return await _familyRepository.InsertAsync(family);
    }

    public async Task<int> UpdateFamilyAsync(Family family)
    {
        family.UpdatedAt = DateTime.Now;
        return await _familyRepository.UpdateAsync(family);
    }

    public async Task<int> DeleteFamilyAsync(int id)
    {
        return await _familyRepository.DeleteAsync(id);
    }
}
