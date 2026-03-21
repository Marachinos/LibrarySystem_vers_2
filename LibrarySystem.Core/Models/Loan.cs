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

    //Förseningsavgift per dag, kan anpassas efter behov
    public decimal LateFeePerDay { get; set; } = 5m;

    public bool IsReturned => ReturnDate.HasValue;

    public bool IsOverdue(DateTime asOf) => !IsReturned && asOf.Date > DueDate.Date;

    public int GetOverdueDays(DateTime asOf)
    {
        if (!IsOverdue(asOf)) return 0;
        return (asOf.Date - DueDate.Date).Days;
    }

    public decimal GetLateFee(DateTime asOf)
    {
        return GetOverdueDays(asOf) * LateFeePerDay;
    }

    public Loan() { }
}