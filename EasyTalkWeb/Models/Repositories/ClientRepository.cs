using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class ClientRepository: GenericRepository<Client>
    {
        public ClientRepository(AppDbContext _context) : base(_context) { }
        public Client GetClientByPersonId(Guid personId)
        {
            // Query the Person entity and include the Client navigation property
            var personWithClient = _context.People
                .Include(p => p.Client)
                .FirstOrDefault(p => p.Id == personId);

            // Access the associated Client entity through the navigation property
            var client = personWithClient?.Client;

            return client;
        }
    }

}
