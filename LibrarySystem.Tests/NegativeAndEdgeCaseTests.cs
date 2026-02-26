using LibrarySystem.Core.Models;
using LibrarySystem.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests;

public class NegativeAndEdgeCaseTests
{
    [Fact]
    public async Task BorrowAsync_ShouldThrow_WhenBookDoesNotExist()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldThrow_WhenBookDoesNotExist));
        var service = new LoanService(ctx);

        var member = new Member { MemberId = "M001", Name = "Anna", Email = "a@a.se" };
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAnyAsync<Exception>(() => service.BorrowAsync(bookId: 999, memberId: member.Id));
    }

    [Fact]
    public async Task BorrowAsync_ShouldThrow_WhenMemberDoesNotExist()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldThrow_WhenMemberDoesNotExist));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "1", Title = "A", Author = "X", PublishedYear = 2000, IsAvailable = true };
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowAsync(book.Id, memberId: 999));

        Assert.Contains("Medlem", ex.Message);
    }

    [Fact]
    public async Task ReturnAsync_ShouldThrow_WhenLoanDoesNotExist()
    {
        using var ctx = TestDbFactory.Create(nameof(ReturnAsync_ShouldThrow_WhenLoanDoesNotExist));
        var service = new LoanService(ctx);

        await Assert.ThrowsAnyAsync<Exception>(() => service.ReturnAsync(loanId: 999));
    }

    [Fact]
    public async Task ReturnAsync_ShouldBeIdempotent_WhenAlreadyReturned()
    {
        using var ctx = TestDbFactory.Create(nameof(ReturnAsync_ShouldBeIdempotent_WhenAlreadyReturned));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "1", Title = "A", Author = "X", PublishedYear = 2000, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Anna", Email = "a@a.se" };
        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var loan = await service.BorrowAsync(book.Id, member.Id);
        await service.ReturnAsync(loan.Id);

        // andra gången ska inte krascha
        await service.ReturnAsync(loan.Id);

        var savedBook = await ctx.Books.FirstAsync(b => b.Id == book.Id);
        Assert.True(savedBook.IsAvailable);
    }
}