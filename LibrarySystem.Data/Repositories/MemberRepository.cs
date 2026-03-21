using LibrarySystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryContext _ctx;

    public MemberRepository(LibraryContext ctx)
    {
        _ctx = ctx;
    }

    public Task<List<Member>> GetAllAsync()
        => _ctx.Members
            .AsNoTracking()
            .OrderBy(m => m.MemberId)
            .ToListAsync();

    public Task<Member?> GetByIdAsync(int id)
        => _ctx.Members
            .Include(m => m.Loans)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

    public Task<Member?> GetByMemberIdAsync(string memberId)
        => _ctx.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MemberId == memberId);

    public async Task AddAsync(Member member)
    {
        var exists = await _ctx.Members.AnyAsync(m => m.MemberId == member.MemberId);
        if (exists)
            throw new InvalidOperationException("MemberId måste vara unik.");

        _ctx.Members.Add(member);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Member member)
    {
        _ctx.Members.Update(member);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _ctx.Members.FindAsync(id);
        if (member is null) return;

        _ctx.Members.Remove(member);
        await _ctx.SaveChangesAsync();
    }

    public Task<List<Member>> SearchAsync(string searchTerm)
    {
        var term = (searchTerm ?? "").Trim();

        return _ctx.Members
            .AsNoTracking()
            .Where(m =>
                m.MemberId.Contains(term) ||
                m.Name.Contains(term) ||
                m.Email.Contains(term))
            .OrderBy(m => m.MemberId)
            .ToListAsync();
    }
}