using LibrarySystem.Core.Models;
using LibrarySystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Tests;

public class LoanRepositoryTests
{
    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyLoansWithoutReturnDate()
    {
        using var ctx = TestDbFactory.Create(nameof(GetActiveAsync_ShouldReturnOnlyLoansWithoutReturnDate));

        var book1 = new Book { ISBN = "111", Title = "Bok 1", Author = "A", PublishedYear = 2020, IsAvailable = false };
        var book2 = new Book { ISBN = "222", Title = "Bok 2", Author = "B", PublishedYear = 2021, IsAvailable = true };
        var member = new Member { MemberId = "M001", Name = "Sandra", Email = "sandra@test.se" };

        ctx.Books.AddRange(book1, book2);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        ctx.Loans.AddRange(
            new Loan
            {
                BookId = book1.Id,
                MemberId = member.Id,
                LoanDate = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(5),
                ReturnDate = null
            },
            new Loan
            {
                BookId = book2.Id,
                MemberId = member.Id,
                LoanDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(-2),
                ReturnDate = DateTime.UtcNow.AddDays(-1)
            });

        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);

        var activeLoans = await repo.GetActiveAsync();

        Assert.Single(activeLoans);
        Assert.Null(activeLoans[0].ReturnDate);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnLoanWithBookAndMember()
    {
        using var ctx = TestDbFactory.Create(nameof(GetByIdAsync_ShouldReturnLoanWithBookAndMember));

        var book = new Book { ISBN = "333", Title = "Testbok", Author = "Test", PublishedYear = 2022, IsAvailable = false };
        var member = new Member { MemberId = "M100", Name = "Anna", Email = "anna@test.se" };

        ctx.Books.Add(book);
        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var loan = new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        ctx.Loans.Add(loan);
        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);

        var result = await repo.GetByIdAsync(loan.Id);

        Assert.NotNull(result);
        Assert.NotNull(result!.Book);
        Assert.NotNull(result.Member);
        Assert.Equal("Testbok", result.Book.Title);
        Assert.Equal("Anna", result.Member.Name);
    }
}