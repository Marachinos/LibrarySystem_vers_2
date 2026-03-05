using LibrarySystem.Data;
using LibrarySystem.Data.Repositories;
using LibrarySystem.Data.Services;
using LibrarySystem.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== EF / SQLite =====
var cs = builder.Configuration.GetConnectionString("LibraryDb")
         ?? "Data Source=library.db";

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite(cs));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<LoanService>();

builder.Services.AddScoped<IBookRepository, BookRepository>();

var app = builder.Build();

// ===== Migrate + Seed =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    db.Database.Migrate();

    // Seed Books
    if (!db.Books.Any())
    {
        db.Books.AddRange(
            new LibrarySystem.Core.Models.Book { ISBN = "123-1", Title = "Sagan om ringen", Author = "J.R.R. Tolkien", PublishedYear = 1954, IsAvailable = true },
            new LibrarySystem.Core.Models.Book { ISBN = "123-2", Title = "Hobbiten", Author = "J.R.R. Tolkien", PublishedYear = 1937, IsAvailable = true },
            new LibrarySystem.Core.Models.Book { ISBN = "123-3", Title = "Clean Code", Author = "Robert C. Martin", PublishedYear = 2008, IsAvailable = true }
        );
        db.SaveChanges();
    }

    //Seed Members
    if (!db.Members.Any())
    {
        db.Members.AddRange(
            new LibrarySystem.Core.Models.Member
            {
                MemberId = "M001",
                Name = "Anna Andersson",
                Email = "anna@test.se",
                MemberSince = DateTime.UtcNow
            },
            new LibrarySystem.Core.Models.Member
            {
                MemberId = "M002",
                Name = "Bertil Berg",
                Email = "bertil@test.se",
                MemberSince = DateTime.UtcNow
            }
        );

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();