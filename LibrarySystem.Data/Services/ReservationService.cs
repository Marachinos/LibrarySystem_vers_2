using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data.Services;

public class ReservationService
{
    private readonly LibraryContext _ctx;

    public ReservationService(LibraryContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Reservation> ReserveAsync(int bookId, int memberId)
    {
        var member = await _ctx.Members.SingleOrDefaultAsync(m => m.Id == memberId);
        if (member is null)
            throw new InvalidOperationException("Medlem finns inte.");

        var book = await _ctx.Books.SingleOrDefaultAsync(b => b.Id == bookId);
        if (book is null)
            throw new InvalidOperationException("Boken finns inte.");

        if (book.IsAvailable)
            throw new InvalidOperationException("Boken är tillgänglig och behöver inte reserveras.");

        var alreadyReserved = await _ctx.Reservations.AnyAsync(r =>
            r.BookId == bookId &&
            r.MemberId == memberId &&
            r.IsActive);

        if (alreadyReserved)
            throw new InvalidOperationException("Medlemmen har redan en aktiv reservation på denna bok.");

        var reservation = new Reservation
        {
            BookId = bookId,
            MemberId = memberId,
            ReservedAt = DateTime.UtcNow,
            IsActive = true
        };

        _ctx.Reservations.Add(reservation);
        await _ctx.SaveChangesAsync();

        return reservation;
    }

    public async Task<List<Reservation>> GetActiveReservationsForBookAsync(int bookId)
    {
        return await _ctx.Reservations
            .Include(r => r.Member)
            .Where(r => r.BookId == bookId && r.IsActive)
            .OrderBy(r => r.ReservedAt)
            .ToListAsync();
    }

    public async Task CancelReservationAsync(int reservationId)
    {
        var reservation = await _ctx.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
        if (reservation is null) return;

        reservation.IsActive = false;
        await _ctx.SaveChangesAsync();
    }
}