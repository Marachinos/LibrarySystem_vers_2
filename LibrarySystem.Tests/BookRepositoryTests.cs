using LibrarySystem.Core.Models;
using LibrarySystem.Data;
using LibrarySystem.Data.Repositories; 
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests
{

    public class BookRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldSaveBookToDatabase()
        {
            using var ctx = TestDbFactory.Create(nameof(AddAsync_ShouldSaveBookToDatabase));
            var repo = new BookRepository(ctx);

            var book = new Book { ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2024 };

            await repo.AddAsync(book);

            var saved = await ctx.Books.FirstOrDefaultAsync(b => b.ISBN == "123");
            Assert.NotNull(saved);
            Assert.Equal("Test", saved!.Title);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBooks()
        {
            using var ctx = TestDbFactory.Create(nameof(GetAllAsync_ShouldReturnAllBooks));
            ctx.Books.AddRange(
                new Book { ISBN = "1", Title = "A", Author = "X", PublishedYear = 2000 },
                new Book { ISBN = "2", Title = "B", Author = "Y", PublishedYear = 2001 }
            );
            await ctx.SaveChangesAsync();

            var repo = new BookRepository(ctx);

            var all = (await repo.GetAllAsync()).ToList();

            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task GetByISBNAsync_ShouldReturnCorrectBook()
        {
            using var ctx = TestDbFactory.Create(nameof(GetByISBNAsync_ShouldReturnCorrectBook));
            ctx.Books.Add(new Book { ISBN = "ABC", Title = "Hit", Author = "A", PublishedYear = 2023 });
            await ctx.SaveChangesAsync();

            var repo = new BookRepository(ctx);

            var found = await repo.GetByISBNAsync("ABC");

            Assert.NotNull(found);
            Assert.Equal("Hit", found!.Title);
        }

        [Fact]
        public async Task SearchAsync_ShouldFindBooksByTitle_And_Author()
        {
            using var ctx = TestDbFactory.Create(nameof(SearchAsync_ShouldFindBooksByTitle_And_Author));
            ctx.Books.AddRange(
                new Book { ISBN = "1", Title = "Sagan om ringen", Author = "Tolkien", PublishedYear = 1954 },
                new Book { ISBN = "2", Title = "Clean Code", Author = "Martin", PublishedYear = 2008 }
            );
            await ctx.SaveChangesAsync();

            var repo = new BookRepository(ctx);

            var hits1 = (await repo.SearchAsync("ringen")).ToList();
            var hits2 = (await repo.SearchAsync("tolkien")).ToList();

            Assert.Single(hits1);
            Assert.Single(hits2);
        }
    }
}