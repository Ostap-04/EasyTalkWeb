namespace EasyTalkWeb.Hubs.Interfaces
{
    public interface IChatClient
    {
        Task ReceiveMessage(Guid senderId, string senderName, string message);

        Task TypingActivity(string senderName);
    }
}
