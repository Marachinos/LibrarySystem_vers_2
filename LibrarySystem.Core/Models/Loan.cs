namespace LibrarySystem.Core.Models;

public class Loan
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = default!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;

    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public bool IsReturned => ReturnDate.HasValue;

    // enkel “overdue” logik (för UI)
    public bool IsOverdue(DateTime asOf) => !IsReturned && asOf.Date > DueDate.Date;

    public Loan() { } // EF
}