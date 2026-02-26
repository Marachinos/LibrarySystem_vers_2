using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryContext _ctx;
    public LoanRepository(LibraryContext ctx) => _ctx = ctx;

    public Task<List<Book>> GetAllAsync()
        => _ctx.Books.AsNoTracking().ToListAsync();

    public Task<Book?> GetByIdAsync(int id)
        => _ctx.Books.Include(b => b.Loans).AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public Task<Book?> GetByISBNAsync(string isbn)
        => _ctx.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

    public async Task AddAsync(Book book)
    {
        _ctx.Books.Add(book);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _ctx.Books.Update(book);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _ctx.Books.FindAsync(id);
        if (book is null) return;
        _ctx.Books.Remove(book);
        await _ctx.SaveChangesAsync();
    }

    public Task<List<Book>> SearchAsync(string searchTerm)
    {
        var term = (searchTerm ?? "").Trim();
        return _ctx.Books.AsNoTracking()
            .Where(b => b.ISBN.Contains(term) || b.Title.Contains(term) || b.Author.Contains(term))
            .ToListAsync();
    }
}