using EasyTalkWeb.Hubs.Interfaces;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.SignalR;

namespace EasyTalkWeb.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly AppDbContext _dbContext;
        public ChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendMessage(string sender, string receiver, string message)
        {
            Console.WriteLine(sender + "  " + receiver + "  " + message);
            var userId = _dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == receiver.ToLower()).Id.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).ReceiveMessage(sender, message);
            }
        }
    }
}
