using LibrarySystem.Core.Models;
using LibrarySystem.Data;
using LibrarySystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests;

public class BookTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var book = new Book("978-91-0-012345-6", "Testbok", "Testförfattare", 2024);

        // Assert
        Assert.Equal("978-91-0-012345-6", book.ISBN);
        Assert.Equal("Testbok", book.Title);
        Assert.Equal("Testförfattare", book.Author);
        Assert.Equal(2024, book.PublishedYear);
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void IsAvailable_ShouldBeTrueForNewBook()
    {
        // Arrange
        var book = new Book("123", "Ny bok", "Någon", 2020);

        // Act & Assert
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void GetInfo_ShouldReturnFormattedString()
    {
        // Arrange
        var book = new Book("123", "Sagan", "Författaren", 1999);

        // Act
        var info = book.GetInfo();

        // Assert
        Assert.Contains("\"Sagan\"", info);
        Assert.Contains("av Författaren", info);
        Assert.Contains("(1999)", info);
        Assert.Contains("ISBN: 123", info);
        Assert.Contains("Tillgänglig", info);
    }
}
