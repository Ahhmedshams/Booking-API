namespace Infrastructure.Identity.EmailSettings
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }
}
