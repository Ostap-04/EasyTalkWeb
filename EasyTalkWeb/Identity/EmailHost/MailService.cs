using EasyTalkWeb.Models;
using EasyTalkWeb.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace EasyTalkWeb.Identity.EmailHost
{
    public class MailService: IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _configuration;
        public MailService(IOptions<MailSettings> mailSettingsOptions, IConfiguration configuration)
        {
            _mailSettings = mailSettingsOptions.Value;
            _configuration = configuration;
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
            string password = _configuration.GetSection("google_auth").GetValue<string>("password");
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
