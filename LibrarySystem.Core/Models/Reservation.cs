namespace LibrarySystem.Core.Models;

public class Reservation
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = default!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = default!;

    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}