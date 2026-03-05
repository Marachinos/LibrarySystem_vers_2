using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _ctx;
    public BookRepository(LibraryContext ctx) => _ctx = ctx;

    public Task<List<Book>> GetAllAsync()
        => _ctx.Books.AsNoTracking().ToListAsync();

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _ctx.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<Book?> GetByISBNAsync(string isbn)
        => _ctx.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

    public async Task AddAsync(Book book)
    {
        _ctx.Books.Add(book);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        var existing = await _ctx.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (existing is null)
            throw new InvalidOperationException("Boken finns inte.");

        existing.ISBN = book.ISBN;
        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.PublishedYear = book.PublishedYear;
        existing.IsAvailable = book.IsAvailable;

        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _ctx.Books.FindAsync(id);
        if (book is null) return;
        _ctx.Books.Remove(book);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<Book>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _ctx.Books.AsNoTracking().ToListAsync();

        var term = searchTerm.Trim().ToLower();

        return await _ctx.Books.AsNoTracking()
            .Where(b =>
                b.Title.ToLower().Contains(term) ||
                b.Author.ToLower().Contains(term) ||
                b.ISBN.ToLower().Contains(term))
            .ToListAsync();
    }
}