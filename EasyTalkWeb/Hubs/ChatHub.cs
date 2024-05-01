using EasyTalkWeb.Hubs.Interfaces;
using EasyTalkWeb.Models;
using EasyTalkWeb.Models.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace EasyTalkWeb.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly MessageRepository _messageRepository;
        public ChatHub(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task SendMessage(Guid chatId, Guid senderId, string senderName, string message)
        {
            await Clients.Group(chatId.ToString()).ReceiveMessage(senderId, senderName, message);
            var newMessage = new Message()
            {
                Id = Guid.NewGuid(),
                ChatId=chatId,
                PersonId = senderId,
                Text = message.Trim(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            await _messageRepository.AddAsync(newMessage);
        }

        public async Task TypingActivity(Guid chatId, string senderName)
        {
            await Clients.OthersInGroup(chatId.ToString()).TypingActivity(senderName);
        }

        public async Task JoinChatRoom(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task LeaveChatRoom(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
            await Console.Out.WriteLineAsync(Context.UserIdentifier + "disconnected");
        }
    }
}
