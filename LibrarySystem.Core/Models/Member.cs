using System.ComponentModel.DataAnnotations;


namespace LibrarySystem.Core.Models;

public class Member
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Medlems-ID är obligatoriskt.")]
    [MaxLength(50)]
    public string MemberId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Namn är obligatoriskt.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-post är obligatorisk.")]
    [EmailAddress(ErrorMessage = "E-postadressen är inte giltig.")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public DateTime MemberSince { get; set; } = DateTime.UtcNow;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public Member() { } // EF

    public Member(string memberId, string name, string email)
    {
        if (string.IsNullOrWhiteSpace(memberId)) throw new ArgumentException("MemberId is required.", nameof(memberId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        MemberId = memberId;
        Name = name;
        Email = email;
        MemberSince = DateTime.UtcNow;
    }
}