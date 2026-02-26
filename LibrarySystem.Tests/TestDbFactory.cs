using LibrarySystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Tests;

public static class TestDbFactory
{
    public static LibraryContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        return new LibraryContext(options);
    }
}