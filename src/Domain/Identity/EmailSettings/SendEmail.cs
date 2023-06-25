using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Infrastructure.Identity.EmailSettings
{
    public class MailService : IMailService
    {
        private readonly MailSettings MailSettings;

        public MailService(IOptions<MailSettings> _MailSettingsOptions)
        {
            MailSettings = _MailSettingsOptions.Value;
        }
        public bool SendMail(MailData mailData)
        {
            try
            {
                MimeMessage EmailMessage = new MimeMessage();

                EmailMessage.From.Add(new MailboxAddress(MailSettings.SenderName, MailSettings.SenderEmail));
                EmailMessage.To.Add(new MailboxAddress(mailData.EmailToName, mailData.EmailTo));

                EmailMessage.Subject = mailData.EmailSubject;
                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = mailData.EmailBody;
                EmailMessage.Body = emailBodyBuilder.ToMessageBody();

                using (SmtpClient MailClient = new SmtpClient())
                {
                    MailClient.Connect(MailSettings.Server, MailSettings.Port, SecureSocketOptions.SslOnConnect);
                    MailClient.Timeout = 6000;
                    MailClient.Authenticate(MailSettings.UserName, MailSettings.Password);
                    MailClient.Send(EmailMessage);
                    MailClient.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
