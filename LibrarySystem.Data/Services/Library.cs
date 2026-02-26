using LibrarySystem.Core.Models;

namespace LibrarySystem.Core.Services;

public class Library
{
    private readonly List<Book> _books = new();
    private readonly List<Member> _members = new();
    private readonly List<Loan> _loans = new();

    public void AddBook(Book book) => _books.Add(book);
    public void AddMember(Member member) => _members.Add(member);

    public int GetTotalBooks() => _books.Count;
    public int GetBorrowedBooksCount() => _books.Count(b => !b.IsAvailable);

    public Member? GetMostActiveBorrower()
        => _loans.GroupBy(l => l.Member)
                 .OrderByDescending(g => g.Count())
                 .Select(g => g.Key)
                 .FirstOrDefault();

    public List<Book> SortBooksByTitle()
        => _books.OrderBy(b => b.Title).ToList();

    public List<Book> SearchBooks(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return new();
        term = term.Trim();
        return _books.Where(b =>
                b.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                b.ISBN.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Om dina tester använder lån:
    public Loan BorrowBook(Book book, Member member, DateTime loanDate, DateTime dueDate)
    {
        book.IsAvailable = false;
        var loan = new Loan { Book = book, Member = member, LoanDate = loanDate, DueDate = dueDate };
        _loans.Add(loan);
        return loan;
    }
}