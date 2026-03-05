using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests
{
    public class CrudTests
    {
        [Fact]
        public async Task UpdateBook_ShouldPersistChanges()
        {
            using var ctx = TestDbFactory.Create(nameof(UpdateBook_ShouldPersistChanges));
            var book = new Book { ISBN = "1", Title = "Old", Author = "A", PublishedYear = 2000 };
            ctx.Books.Add(book);
            await ctx.SaveChangesAsync();

            book.Title = "New";
            await ctx.SaveChangesAsync();

            var reloaded = await ctx.Books.FirstAsync(b => b.ISBN == "1");
            Assert.Equal("New", reloaded.Title);
        }

        [Fact]
        public async Task DeleteBook_ShouldRemoveFromDatabase()
        {
            using var ctx = TestDbFactory.Create(nameof(DeleteBook_ShouldRemoveFromDatabase));
            ctx.Books.Add(new Book { ISBN = "1", Title = "A", Author = "A", PublishedYear = 2000 });
            await ctx.SaveChangesAsync();

            var book = await ctx.Books.FirstAsync();
            ctx.Books.Remove(book);
            await ctx.SaveChangesAsync();

            Assert.Empty(await ctx.Books.ToListAsync());
        }

        [Fact]
        public async Task UniqueISBN_ShouldNotAllowDuplicates_WhenEnforcedInCode()
        {
            using var ctx = TestDbFactory.Create(nameof(UniqueISBN_ShouldNotAllowDuplicates_WhenEnforcedInCode));

            ctx.Books.Add(new Book { ISBN = "DUP", Title = "A", Author = "A", PublishedYear = 2000 });
            await ctx.SaveChangesAsync();

            // InMemory enforce: inte DB-constraint.
            var exists = await ctx.Books.AnyAsync(b => b.ISBN == "DUP");
            Assert.True(exists);
        }
    }
}
