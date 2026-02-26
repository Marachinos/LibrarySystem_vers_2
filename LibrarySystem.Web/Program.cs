using LibrarySystem.Data;
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

var app = builder.Build();

// ===== (Valfritt men rekommenderat) Migrate + Seed =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    db.Database.Migrate();

    // Seed om tomt (du kan ta bort detta senare)
    if (!db.Books.Any())
    {
        db.Books.AddRange(
            new LibrarySystem.Core.Models.Book { ISBN = "123-1", Title = "Sagan om ringen", Author = "J.R.R. Tolkien", PublishedYear = 1954, IsAvailable = true },
            new LibrarySystem.Core.Models.Book { ISBN = "123-2", Title = "Hobbiten", Author = "J.R.R. Tolkien", PublishedYear = 1937, IsAvailable = true },
            new LibrarySystem.Core.Models.Book { ISBN = "123-3", Title = "Clean Code", Author = "Robert C. Martin", PublishedYear = 2008, IsAvailable = true }
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