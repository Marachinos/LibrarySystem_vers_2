using Bunit;
using LibrarySystem.Core.Models;
using LibrarySystem.Data;
using LibrarySystem.Web.Components.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibrarySystem.Tests;

public class HomeTests : TestContext
{
    [Fact]
    public void Home_ShouldRenderStatistics()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new LibraryContext(options);

        db.Books.AddRange(
            new Book { ISBN = "1", Title = "A", Author = "AA", PublishedYear = 2020 },
            new Book { ISBN = "2", Title = "B", Author = "BB", PublishedYear = 2021 });

        var member = new Member
        {
            MemberId = "M001",
            Name = "Sandra",
            Email = "sandra@test.se"
        };

        db.Members.Add(member);
        db.SaveChanges();

        db.Loans.Add(new Loan
        {
            BookId = 1,
            MemberId = member.Id,
            LoanDate = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(-1),
            ReturnDate = null
        });

        db.SaveChanges();

        Services.AddSingleton(db);

        var cut = RenderComponent<Home>();

        cut.Markup.Contains("Antal böcker");
        cut.Markup.Contains("Antal medlemmar");
        cut.Markup.Contains("Aktiva lån");
        cut.Markup.Contains("Försenade lån");

        cut.Markup.Contains(">2<");
        cut.Markup.Contains(">1<");
    }
}