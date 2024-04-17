namespace EasyTalkWeb.Identity.EmailHost
{
    public interface IMailService
    {
        bool SendEmail(string userEmail, string confirmationLink);
    }

}
