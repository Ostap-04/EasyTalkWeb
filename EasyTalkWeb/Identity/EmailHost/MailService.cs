using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EasyTalkWeb.Identity.EmailHost
{
    public class MailService: IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }

        public bool SendEmail(string userEmail, string confirmationLink, string mailTitle)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_mailSettings.SenderEmail);
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = mailTitle;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationLink;

            SmtpClient client = new SmtpClient(_mailSettings.Server);
            string password = Environment.GetEnvironmentVariable(_mailSettings.UserName);
            client.Credentials = new NetworkCredential(_mailSettings.UserName, password);
            client.Port = _mailSettings.Port;
            client.EnableSsl = true;
            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }



    }
}
