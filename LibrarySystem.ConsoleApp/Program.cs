using LibrarySystem.Core.Models;
using LibrarySystem.Data;
using LibrarySystem.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
    })
    .ConfigureServices(services =>
    {
        services.AddDbContext<LibraryContext>(options =>
            options.UseSqlite("Data Source=library_console.db"));

        services.AddScoped<LoanService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
var loanService = scope.ServiceProvider.GetRequiredService<LoanService>();

Console.WriteLine("Kör migrations (Database.Migrate)...");
await db.Database.MigrateAsync();
Console.WriteLine("OK.");
Console.WriteLine("====================================\n");

// Seed om tomt
if (!await db.Books.AnyAsync())
{
    db.Books.AddRange(
        new Book { ISBN = "123-1", Title = "Sagan om ringen", Author = "J.R.R. Tolkien", PublishedYear = 1954, IsAvailable = true },
        new Book { ISBN = "123-2", Title = "Hobbiten", Author = "J.R.R. Tolkien", PublishedYear = 1937, IsAvailable = true },
        new Book { ISBN = "123-3", Title = "Clean Code", Author = "Robert C. Martin", PublishedYear = 2008, IsAvailable = true }
    );
}

if (!await db.Members.AnyAsync())
{
    db.Members.AddRange(
        new Member { MemberId = "M001", Name = "Anna Andersson", Email = "anna@test.se", MemberSince = DateTime.UtcNow },
        new Member { MemberId = "M002", Name = "Bertil Berg", Email = "bertil@test.se", MemberSince = DateTime.UtcNow }
    );
}

await db.SaveChangesAsync();

while (true)
{
    Console.WriteLine("\n==== Library System Console App (SQLite) ====");
    Console.WriteLine("1. Lista böcker");
    Console.WriteLine("2. Lista medlemmar");
    Console.WriteLine("3. Låna bok (bookId, memberId)");
    Console.WriteLine("4. Returnera lån (loanId)");
    Console.WriteLine("5. Lista aktiva lån");
    Console.WriteLine("0. Avsluta");
    Console.Write("Välj: ");

    var choice = Console.ReadLine();
    if (choice == "0") break;

    switch (choice)
    {
        case "1":
            {
                var books = await db.Books.AsNoTracking().OrderBy(b => b.Id).ToListAsync();
                Console.WriteLine("\nBöcker:");
                foreach (var b in books)
                    Console.WriteLine($"{b.Id}. {b.GetInfo()}");
                break;
            }

        case "2":
            {
                var members = await db.Members.AsNoTracking().OrderBy(m => m.Id).ToListAsync();
                Console.WriteLine("\nMedlemmar:");
                foreach (var m in members)
                    Console.WriteLine($"{m.Id}. {m.MemberId} - {m.Name} ({m.Email})");
                break;
            }

        case "3":
            {
                Console.Write("BookId: ");
                if (!int.TryParse(Console.ReadLine(), out var bookId))
                {
                    Console.WriteLine("Ogiltigt BookId.");
                    break;
                }

                Console.Write("MemberId: ");
                if (!int.TryParse(Console.ReadLine(), out var memberId))
                {
                    Console.WriteLine("Ogiltigt MemberId.");
                    break;
                }

                try
                {
                    var loan = await loanService.BorrowAsync(bookId, memberId, days: 14);
                    Console.WriteLine($"OK! Skapade lån #{loan.Id}. Förfallodatum: {loan.DueDate:yyyy-MM-dd}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fel: {ex.Message}");
                }

                break;
            }

        case "4":
            {
                Console.Write("LoanId: ");
                if (!int.TryParse(Console.ReadLine(), out var loanId))
                {
                    Console.WriteLine("Ogiltigt LoanId.");
                    break;
                }

                try
                {
                    await loanService.ReturnAsync(loanId);
                    Console.WriteLine("OK! Returnerad.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fel: {ex.Message}");
                }

                break;
            }

        case "5":
            {
                var active = await db.Loans
                    .AsNoTracking()
                    .Where(l => l.ReturnDate == null)
                    .OrderBy(l => l.Id)
                    .ToListAsync();

                Console.WriteLine("\nAktiva lån:");
                if (active.Count == 0)
                {
                    Console.WriteLine("(Inga aktiva lån)");
                    break;
                }

                foreach (var l in active)
                    Console.WriteLine($"{l.Id}. BookId={l.BookId}, MemberId={l.MemberId}, Due={l.DueDate:yyyy-MM-dd}");

                break;
            }

        default:
            Console.WriteLine("Okänt val.");
            break;
    }
}

Console.WriteLine("Hejdå!");