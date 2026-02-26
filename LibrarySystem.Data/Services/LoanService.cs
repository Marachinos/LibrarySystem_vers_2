using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data.Services;

public class LoanService
{
    private readonly LibraryContext _ctx;
    public LoanService(LibraryContext ctx) => _ctx = ctx;

    public async Task<Loan> BorrowAsync(int bookId, int memberId, int days = 14)
    {
        var member = await _ctx.Members.SingleOrDefaultAsync(m => m.Id == memberId);
        if (member is null)
            throw new InvalidOperationException("Medlem finns inte.");

        var book = await _ctx.Books.SingleOrDefaultAsync(b => b.Id == bookId);
        if (book is null)
            throw new InvalidOperationException("Boken finns inte.");

        if (!book.IsAvailable)
            throw new InvalidOperationException("Boken är redan utlånad.");

        book.IsAvailable = false;

        var now = DateTime.UtcNow;
        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            LoanDate = now,
            DueDate = now.AddDays(days)
        };

        _ctx.Loans.Add(loan);
        await _ctx.SaveChangesAsync();
        return loan;
    }

    public async Task ReturnAsync(int loanId)
    {
        var loan = await _ctx.Loans.Include(l => l.Book).FirstAsync(l => l.Id == loanId);
        if (loan.ReturnDate is not null) return;

        loan.ReturnDate = DateTime.UtcNow;
        loan.Book.IsAvailable = true;

        await _ctx.SaveChangesAsync();
    }

    public Task<int> ActiveLoansCountAsync()
        => _ctx.Loans.CountAsync(l => l.ReturnDate == null);
}