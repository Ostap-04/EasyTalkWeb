using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class ChatRepository : GenericRepository<Chat>
    {
        public ChatRepository(AppDbContext _context) : base(_context)
        {
        }

        public async Task<ICollection<Chat>> GetChatsForPersonAsync(Guid personId)
        {
            var chats = await _context.Chats
                .Include(c => c.Persons)
                .Where(c => c.Persons.Any(p => p.Id == personId))
                .ToListAsync();
            return chats;
        }

        public virtual async Task<ICollection<Chat>> GetChatsWithMsgsForPersonAsync(Guid personId)
        {
            var chats = await _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.Persons)
                .Where(c => c.Persons.Any(p => p.Id == personId))
                .ToListAsync();
            return chats;
        }

        public virtual async Task<Chat> GetChatWithMsgsAsync(Guid chatId)
        {
            var chat = await _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.Persons)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            return chat;
        }

        public async Task<Chat> GetChatWithJobPostAsync(Guid chatId)
        {
            var chat = await _context.Chats
                .Include(c => c.JobPost)
                .FirstOrDefaultAsync(c => c.Id == chatId);
            return chat;
        }
    }
}
