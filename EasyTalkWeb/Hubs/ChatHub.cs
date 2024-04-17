using EasyTalkWeb.Hubs.Interfaces;
using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EasyTalkWeb.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly UserManager<Person> _userManager;
        private readonly MessageRepository _repo;
        public ChatHub(UserManager<Person> userManager, MessageRepository repo)
        {
            _userManager = userManager;
            _repo = repo;
        }

        public async Task SendMessage(string sender, string receiver, string message)
        {
            Console.WriteLine(sender + "  " + receiver + "  " + message);
            await Console.Out.WriteLineAsync(Context.UserIdentifier);
            var userId = _userManager.Users.FirstOrDefault(u => u.Email.ToLower() == receiver.ToLower()).Id.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).ReceiveMessage(sender, message);
            }
        }
    }
}
