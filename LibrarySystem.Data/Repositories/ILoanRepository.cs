using LibrarySystem.Core.Models;

namespace LibrarySystem.Data.Repositories;

public interface ILoanRepository
{
    Task<List<Loan>> GetAllAsync();
    Task<List<Loan>> GetActiveAsync();
    Task<Loan?> GetByIdAsync(int id);
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task DeleteAsync(int id);
}