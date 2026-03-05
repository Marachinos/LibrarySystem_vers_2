using LibrarySystem.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Core.Models;

public class Book : ISearchable
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ISBN är obligatoriskt.")]
    [MaxLength(50)]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Titel är obligatorisk.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Författare är obligatorisk.")]
    [MaxLength(200)]
    public string Author { get; set; } = string.Empty;

    [Range(1, 3000, ErrorMessage = "Publiceringsår måste vara ett giltigt år.")]
    public int PublishedYear { get; set; }

    public bool IsAvailable { get; set; } = true;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public Book() { } // EF

    public Book(string isbn, string title, string author, int publishedYear)
    {
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN is required.", nameof(isbn));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author is required.", nameof(author));
        if (publishedYear < 0) throw new ArgumentOutOfRangeException(nameof(publishedYear));

        ISBN = isbn;
        Title = title;
        Author = author;
        PublishedYear = publishedYear;
        IsAvailable = true;
    }

    public string GetInfo()
    {
        var status = IsAvailable ? "Tillgänglig" : "Utlånad";
        return $"\"{Title}\" av {Author} ({PublishedYear}) - ISBN: {ISBN} - {status}";
    }

    public bool Matches(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return false;
        var term = searchTerm.Trim();

        return ISBN.Contains(term, StringComparison.OrdinalIgnoreCase)
            || Title.Contains(term, StringComparison.OrdinalIgnoreCase)
            || Author.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}