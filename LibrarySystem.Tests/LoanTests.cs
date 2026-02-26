using LibrarySystem.Core.Models;
using LibrarySystem.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests;

public class LoanTests
{
    [Fact]
    public async Task BorrowAsync_ShouldCreateLoan_AndSetBookUnavailable()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldCreateLoan_AndSetBookUnavailable));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "123", Title = "Test", Author = "A", PublishedYear = 2024, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Anna", Email = "anna@test.se" };

        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var loan = await service.BorrowAsync(book.Id, member.Id, days: 14);

        Assert.NotNull(loan);
        var savedBook = await ctx.Books.FirstAsync(b => b.Id == book.Id);
        Assert.False(savedBook.IsAvailable);

        var savedLoan = await ctx.Loans.FirstOrDefaultAsync(l => l.Id == loan.Id);
        Assert.NotNull(savedLoan);
        Assert.Equal(book.Id, savedLoan!.BookId);
        Assert.Equal(member.Id, savedLoan.MemberId);
        Assert.Null(savedLoan.ReturnDate);
    }

    [Fact]
    public async Task BorrowAsync_ShouldThrow_WhenBookAlreadyBorrowed()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldThrow_WhenBookAlreadyBorrowed));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "123", Title = "Test", Author = "A", PublishedYear = 2024, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Anna", Email = "anna@test.se" };
        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        await service.BorrowAsync(book.Id, member.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.BorrowAsync(book.Id, member.Id));
    }

    [Fact]
    public async Task ReturnAsync_ShouldSetReturnDate_AndSetBookAvailable()
    {
        using var ctx = TestDbFactory.Create(nameof(ReturnAsync_ShouldSetReturnDate_AndSetBookAvailable));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "123", Title = "Test", Author = "A", PublishedYear = 2024, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Anna", Email = "anna@test.se" };
        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var loan = await service.BorrowAsync(book.Id, member.Id);
        await service.ReturnAsync(loan.Id);

        var savedLoan = await ctx.Loans.FirstAsync(l => l.Id == loan.Id);
        Assert.NotNull(savedLoan.ReturnDate);

        var savedBook = await ctx.Books.FirstAsync(b => b.Id == book.Id);
        Assert.True(savedBook.IsAvailable);
    }

    [Fact]
    public async Task BorrowAsync_ShouldNotCreateSecondActiveLoan_ForSameBook()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldNotCreateSecondActiveLoan_ForSameBook));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "123", Title = "Test", Author = "A", PublishedYear = 2024, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Anna", Email = "anna@test.se" };
        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        // Första lånet går igenom
        await service.BorrowAsync(book.Id, member.Id);

        // Andra försöket ska kasta
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowAsync(book.Id, member.Id));

        // Och det ska INTE ha skapats en extra lånerad
        var loansForBook = await ctx.Loans.CountAsync(l => l.BookId == book.Id);
        Assert.Equal(1, loansForBook);

        // Och exakt 1 aktivt lån (ReturnDate null)
        var activeLoans = await ctx.Loans.CountAsync(l => l.BookId == book.Id && l.ReturnDate == null);
        Assert.Equal(1, activeLoans);
    }

    [Fact]
    public async Task BorrowAsync_ShouldAllowBorrowAgain_AfterReturn()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowAsync_ShouldAllowBorrowAgain_AfterReturn));
        var service = new LoanService(ctx);

        var book = new Book { ISBN = "123", Title = "Test", Author = "A", PublishedYear = 2024, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Anna", Email = "anna@test.se" };
        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var loan1 = await service.BorrowAsync(book.Id, member.Id);
        await service.ReturnAsync(loan1.Id);

        var loan2 = await service.BorrowAsync(book.Id, member.Id);

        Assert.NotEqual(loan1.Id, loan2.Id);

        var activeLoans = await ctx.Loans.CountAsync(l => l.BookId == book.Id && l.ReturnDate == null);
        Assert.Equal(1, activeLoans);
    }
}