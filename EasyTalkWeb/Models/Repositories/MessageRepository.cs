using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class MessageRepository: GenericRepository<Message>
    {
        public MessageRepository(AppDbContext _context) : base(_context) { }
    }
}
