using LibrarySystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDbContext<LibraryContext>(options =>
    options.UseMySql("server=localhost;database=librarysystem;user=root;password=Lilleman2026;",
        ServerVersion.AutoDetect("server=localhost;database=librarysystem;user=root;password=Lilleman2026;")));

var provider = services.BuildServiceProvider();
using var db = provider.GetRequiredService<LibraryContext>();

var books = await db.Books.ToListAsync();
foreach (var b in books)
    Console.WriteLine(b.GetInfo());