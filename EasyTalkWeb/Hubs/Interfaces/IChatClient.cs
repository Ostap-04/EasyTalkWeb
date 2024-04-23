namespace EasyTalkWeb.Hubs.Interfaces
{
    public interface IChatClient
    {
        Task ReceiveMessage(string sender, string message);
    }
}
