using Laba_3.Models;

namespace Laba_3.Interfaces;

public interface IDiscussionRepository
{
    Task<IEnumerable<Discussion>> GetAllDiscussionsAsync();
    Task<Discussion> GetDiscussionByIdAsync(int id);
    Task AddDiscussionAsync(Discussion discussion);
    Task UpdateDiscussionAsync(Discussion discussion); 
    Task DeleteDiscussionAsync(int id);
}
