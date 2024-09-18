using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace UserService.Services
{
    public class EmailSender:IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("enchantedbookshelf8@gmail.com", "cteesiyjdnccwckm"),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("enchantedbookshelf8@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
