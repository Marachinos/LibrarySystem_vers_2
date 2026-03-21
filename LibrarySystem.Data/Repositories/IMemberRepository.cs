using LibrarySystem.Core.Models;

namespace LibrarySystem.Data.Repositories;

public interface IMemberRepository
{
    Task<List<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(int id);
    Task<Member?> GetByMemberIdAsync(string memberId);
    Task AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(int id);
    Task<List<Member>> SearchAsync(string searchTerm);
}