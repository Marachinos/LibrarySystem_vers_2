using LibrarySystem.Core.Models;
using LibrarySystem.Data.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests;

public class LibraryStatisticsTests
{
    [Fact]
    public async Task TotalBooks_ShouldReturnCorrectCount()
    {
        using var ctx = TestDbFactory.Create(nameof(TotalBooks_ShouldReturnCorrectCount));

        ctx.Books.AddRange(
            new Book { ISBN = "1", Title = "A", Author = "X", PublishedYear = 2000 },
            new Book { ISBN = "2", Title = "B", Author = "Y", PublishedYear = 2001 }
        );

        await ctx.SaveChangesAsync();

        var total = await ctx.Books.CountAsync();
        Assert.Equal(2, total);
    }

    [Fact]
    public async Task BorrowedBooks_ShouldReturnCorrectCount()
    {
        using var ctx = TestDbFactory.Create(nameof(BorrowedBooks_ShouldReturnCorrectCount));

        ctx.Books.AddRange(
            new Book { ISBN = "1", Title = "A", Author = "X", PublishedYear = 2000, IsAvailable = true },
            new Book { ISBN = "2", Title = "B", Author = "Y", PublishedYear = 2001, IsAvailable = false },
            new Book { ISBN = "3", Title = "C", Author = "Z", PublishedYear = 2002, IsAvailable = false }
        );

        await ctx.SaveChangesAsync();

        var borrowed = await ctx.Books.CountAsync(b => !b.IsAvailable);
        Assert.Equal(2, borrowed);
    }

    [Fact]
    public async Task MostActiveBorrower_ShouldReturnMemberWithMostLoans()
    {
        using var ctx = TestDbFactory.Create(nameof(MostActiveBorrower_ShouldReturnMemberWithMostLoans));
        var service = new LoanService(ctx);

        var m1 = new Member { MemberId = "M001", Name = "Anna", Email = "a@a.se" };
        var m2 = new Member { MemberId = "M002", Name = "Bertil", Email = "b@b.se" };

        var b1 = new Book { ISBN = "1", Title = "A", Author = "X", PublishedYear = 2000, IsAvailable = true };
        var b2 = new Book { ISBN = "2", Title = "B", Author = "Y", PublishedYear = 2001, IsAvailable = true };
        var b3 = new Book { ISBN = "3", Title = "C", Author = "Z", PublishedYear = 2002, IsAvailable = true };

        ctx.Members.AddRange(m1, m2);
        ctx.Books.AddRange(b1, b2, b3);
        await ctx.SaveChangesAsync();

        await service.BorrowAsync(b1.Id, m1.Id);
        await service.BorrowAsync(b2.Id, m1.Id);
        await service.BorrowAsync(b3.Id, m2.Id);

        // statistik via Loans-tabellen
        var mostActiveMemberId = await ctx.Loans
            .GroupBy(l => l.MemberId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstAsync();

        var mostActive = await ctx.Members.FirstAsync(m => m.Id == mostActiveMemberId);
        Assert.Equal("M001", mostActive.MemberId);
    }
}