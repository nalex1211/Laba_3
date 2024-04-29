using Microsoft.EntityFrameworkCore;
using Laba_3.Data;
using Laba_3.Interfaces;
using Laba_3.Models;

public class DiscussionRepository : IDiscussionRepository
{
    private readonly ApplicationDbContext _db;

    public DiscussionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Discussion>> GetAllDiscussionsAsync()
    {
        return await _db.Discussions.Include(d => d.Comments).ToListAsync();
    }

    public async Task<Discussion> GetDiscussionByIdAsync(int id)
    {
        return await _db.Discussions.Include(d => d.Comments).FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddDiscussionAsync(Discussion discussion)
    {
        await _db.Discussions.AddAsync(discussion);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateDiscussionAsync(Discussion discussion)
    {
        _db.Discussions.Update(discussion);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteDiscussionAsync(int id)
    {
        var discussion = await GetDiscussionByIdAsync(id);
        if (discussion != null)
        {
            _db.Discussions.Remove(discussion);
            await _db.SaveChangesAsync();
        }
    }
}
