using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class ClientRepository: GenericRepository<Client>
    {
        public ClientRepository(AppDbContext _context) : base(_context) { }

        public Client GetClientByPersonId(Guid personId)
        {
            var personWithClient = _context.People
                .Include(p => p.Client)
                .FirstOrDefault(p => p.Id == personId);

            var client = personWithClient?.Client;

            return client;
        }
    }
}
