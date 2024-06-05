using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class MessageRepository: GenericRepository<Message>
    {
        public MessageRepository(AppDbContext _context) : base(_context) { }

        public async Task<ICollection<Message>> GetChatMessagesAsync(Guid chatId)
        {
            var messages = await _context.Messages.Where(m => m.ChatId == chatId).ToListAsync();
            return messages;
        }
    }
}
