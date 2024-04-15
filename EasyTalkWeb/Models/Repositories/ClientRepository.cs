using EasyTalkWeb.Persistance;

namespace EasyTalkWeb.Models.Repositories
{
    public class ClientRepository: GenericRepository<Client>
    {
        public ClientRepository(AppDbContext _context) : base(_context) { }
    }
}
