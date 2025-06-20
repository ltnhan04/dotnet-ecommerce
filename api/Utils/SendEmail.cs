using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace api.Utils
{
    public class SendEmail
    {
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _emailUsername = Environment.GetEnvironmentVariable("EMAIL_USERNAME")!;
        private readonly string _emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")!;
        private readonly string _emailFrom = "iTribe.huflit@gmail.com";
        private readonly string _displayName = "iTribe.huflit";
        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailUsername, _emailPassword);

                var mail = new MailMessage
                {
                    From = new MailAddress(_emailFrom, _displayName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                };
                mail.To.Add(to);
                await client.SendMailAsync(mail);
            }
        }
    }
}