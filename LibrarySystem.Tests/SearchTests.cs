using LibrarySystem.Core.Models;
using LibrarySystem.Data;
using LibrarySystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibrarySystem.Tests; //trodde att man skulle ha {}, men det funkar visst såhär också

public class SearchTests
{
    [Theory]
    [InlineData("Tolkien", true)]
    [InlineData("tolkien", true)] // Case-insensitive?
    [InlineData("Rowling", false)]
    public void Book_Matches_ShouldFindByAuthor(string searchTerm, bool expected)
    {
        // Arrange
        var book = new Book("123", "Sagan om ringen", "J.R.R. Tolkien", 1954);

        // Act
        var result = book.Matches(searchTerm);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Book_Matches_ShouldFindByIsbn()
    {
        // Arrange
        var book = new Book("ABC-999", "Titel", "Författare", 2000);

        // Act & Assert
        Assert.True(book.Matches("abc-999")); // case-insensitive
    }
}