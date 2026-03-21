using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryContext _ctx;

    public LoanRepository(LibraryContext ctx)
    {
        _ctx = ctx;
    }

    public Task<List<Loan>> GetAllAsync()
        => _ctx.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsNoTracking()
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();

    public Task<List<Loan>> GetActiveAsync()
        => _ctx.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsNoTracking()
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

    public Task<Loan?> GetByIdAsync(int id)
        => _ctx.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task AddAsync(Loan loan)
    {
        _ctx.Loans.Add(loan);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Loan loan)
    {
        _ctx.Loans.Update(loan);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var loan = await _ctx.Loans.FindAsync(id);
        if (loan is null) return;

        _ctx.Loans.Remove(loan);
        await _ctx.SaveChangesAsync();
    }
}