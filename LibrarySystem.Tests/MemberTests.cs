using LibrarySystem.Core.Models;
using LibrarySystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests;

public class MemberTests
{
    [Fact]
    public async Task AddMember_ShouldSaveMember()
    {
        using var ctx = TestDbFactory.Create(nameof(AddMember_ShouldSaveMember));

        var member = new Member
        {
            MemberId = "M001",
            Name = "Anna Andersson",
            Email = "anna@test.se",
            MemberSince = DateTime.UtcNow
        };

        ctx.Members.Add(member);
        await ctx.SaveChangesAsync();

        var saved = await ctx.Members.FirstOrDefaultAsync(m => m.MemberId == "M001");
        Assert.NotNull(saved);
        Assert.Equal("Anna Andersson", saved!.Name);
    }

    [Fact]
    public async Task Member_ShouldHaveZeroLoansInitially()
    {
        using var ctx = TestDbFactory.Create(nameof(Member_ShouldHaveZeroLoansInitially));

        ctx.Members.Add(new Member { MemberId = "M001", Name = "Anna", Email = "a@a.se" });
        await ctx.SaveChangesAsync();

        var member = await ctx.Members.Include(m => m.Loans).FirstAsync(m => m.MemberId == "M001");
        Assert.Empty(member.Loans);
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenMemberIdAlreadyExists()
    {
        using var ctx = TestDbFactory.Create(nameof(AddAsync_ShouldThrow_WhenMemberIdAlreadyExists));
        var repo = new MemberRepository(ctx);

        await repo.AddAsync(new Member { MemberId = "M001", Name = "Anna", Email = "a@a.se" });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.AddAsync(new Member { MemberId = "M001", Name = "Bertil", Email = "b@b.se" }));
    }
}