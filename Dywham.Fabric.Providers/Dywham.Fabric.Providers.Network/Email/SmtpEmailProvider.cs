using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Network.Email
{
    public class SmtpEmailProvider : IEmailProvider
    {
        public IEmailProviderSettings EmailProviderSettings { get; set; }


        public async Task SendEmailAsync(string subject, string body, bool isBodyHtml, string destination, CancellationToken token)
        {
            await SendEmailAsync(new SenderDetails
            {
                EmailProviderFrom = EmailProviderSettings.EmailProviderFrom,
                EmailProviderUsername = EmailProviderSettings.EmailProviderUsername,
                EmailProviderPassword = EmailProviderSettings.EmailProviderPassword,
                EmailProviderHost = EmailProviderSettings.EmailProviderHost,
                EmailProviderPort = EmailProviderSettings.EmailProviderPort,
                EmailProviderUseSsl = EmailProviderSettings.EmailProviderUseSsl,
                EmailProviderFromDisplay = EmailProviderSettings.EmailProviderFromDisplay,
                UseDefaultCredentials = EmailProviderSettings.UseDefaultCredentials
            }, subject, body, isBodyHtml, destination, token);
        }

        public async Task SendEmailAsync(SenderDetails details, string subject, string body, bool isBodyHtml, string destination, CancellationToken token)
        {
            var message = new MailMessage
            {
                From = new MailAddress(details.EmailProviderFrom, details.EmailProviderFromDisplay)
            };

            message.To.Add(destination);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;

            var smtp = new SmtpClient
            {
                Host = details.EmailProviderHost,
                EnableSsl = details.EmailProviderUseSsl
            };

            var networkCred = new System.Net.NetworkCredential
            {
                UserName = details.EmailProviderUsername,
                Password = details.EmailProviderPassword
            };

            smtp.UseDefaultCredentials = details.UseDefaultCredentials;
            smtp.Credentials = networkCred;
            smtp.Port = details.EmailProviderPort;

            await smtp.SendMailAsync(message);
        }
    }
}