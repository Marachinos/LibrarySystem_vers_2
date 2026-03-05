using LibrarySystem.Core.Models;
using LibrarySystem.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests
{
    public class LoanServiceIntegrationTests
    {
        [Fact]
        public async Task BorrowAsync_ShouldCreateLoan_And_SetBookUnavailable()
        {
            using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldCreateLoan_And_SetBookUnavailable));
            var book = new Book { ISBN = "1", Title = "A", Author = "A", PublishedYear = 2000, IsAvailable = true };
            var member = new Member { MemberId = "M1", Name = "Anna", Email = "a@test.se", MemberSince = DateTime.UtcNow };
            ctx.Books.Add(book);
            ctx.Members.Add(member);
            await ctx.SaveChangesAsync();

            var service = new LoanService(ctx);

            var loan = await service.BorrowAsync(book.Id, member.Id);

            Assert.NotNull(loan);
            var updatedBook = await ctx.Books.FirstAsync(b => b.Id == book.Id);
            Assert.False(updatedBook.IsAvailable);
            Assert.Equal(book.Id, loan.BookId);
            Assert.Equal(member.Id, loan.MemberId);
        }

        [Fact]
        public async Task ReturnAsync_ShouldSetReturnDate_And_SetBookAvailable()
        {
            using var ctx = TestDbFactory.Create(nameof(ReturnAsync_ShouldSetReturnDate_And_SetBookAvailable));
            var book = new Book { ISBN = "1", Title = "A", Author = "A", PublishedYear = 2000, IsAvailable = true };
            var member = new Member { MemberId = "M1", Name = "Anna", Email = "a@test.se", MemberSince = DateTime.UtcNow };
            ctx.Books.Add(book);
            ctx.Members.Add(member);
            await ctx.SaveChangesAsync();

            var service = new LoanService(ctx);
            var loan = await service.BorrowAsync(book.Id, member.Id);

            await service.ReturnAsync(loan.Id);

            var reloadedLoan = await ctx.Loans.FirstAsync(l => l.Id == loan.Id);
            Assert.NotNull(reloadedLoan.ReturnDate);

            var updatedBook = await ctx.Books.FirstAsync(b => b.Id == book.Id);
            Assert.True(updatedBook.IsAvailable);
        }

        [Fact]
        public async Task BorrowAsync_ShouldThrow_WhenBookAlreadyBorrowed()
        {
            using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldThrow_WhenBookAlreadyBorrowed));
            var book = new Book { ISBN = "1", Title = "A", Author = "A", PublishedYear = 2000, IsAvailable = true };
            var member = new Member { MemberId = "M1", Name = "Anna", Email = "a@test.se", MemberSince = DateTime.UtcNow };
            ctx.Books.Add(book);
            ctx.Members.Add(member);
            await ctx.SaveChangesAsync();

            var service = new LoanService(ctx);
            await service.BorrowAsync(book.Id, member.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.BorrowAsync(book.Id, member.Id));
        }
    }
}
