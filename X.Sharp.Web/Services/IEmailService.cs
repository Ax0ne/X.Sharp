namespace X.Sharp.Web.Services
{
    public interface IEmailService
    {
        void Send(EmailMessage email);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
