using Microsoft.AspNetCore.SignalR;

// WTF : https://github.com/Azure/azure-signalr/issues/1233
namespace EasyTalkWeb.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId);
        }
        public async Task SendMessage(string user, string message)
        {
            await Console.Out.WriteLineAsync(Context.UserIdentifier.ToString());
            await Console.Out.WriteLineAsync(user + " : " + message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
