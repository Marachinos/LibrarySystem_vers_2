using LibrarySystem.Core.Models;

namespace LibrarySystem.Data.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book?> GetByISBNAsync(string isbn);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task<List<Book>> SearchAsync(string searchTerm);
}