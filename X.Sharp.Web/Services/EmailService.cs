// Copyright (c) Ax0ne.  All Rights Reserved

using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace X.Sharp.Web.Services
{
    [Inject(typeof(IEmailService))]
    public class EmailService : IEmailService
    {
        public void Send(EmailMessage email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(email.SenderName, email.SenderEmail));
            message.To.Add(new MailboxAddress(email.ReceiveName, email.ReceiveEmail));
            message.Subject = email.Subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = email.Content
            };
            using var emailClient = new SmtpClient();
            emailClient.Connect("smtp.office365.com", 587, SecureSocketOptions.Auto);
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            emailClient.Authenticate("xxx@outlook.com", "xxx");
            emailClient.Send(message);
            emailClient.Disconnect(true);
        }

        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            using var emailClient = new Pop3Client();
            emailClient.Connect("POP3 server name", 995, SecureSocketOptions.Auto);
            //Remove any OAuth functionality as we won't be using it.
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            emailClient.Authenticate("POP3 server username", "POP3 server password");
            List<EmailMessage> emails = new List<EmailMessage>();
            for (int i = 0; i < emailClient.Count && i < maxCount; i++)
            {
                var message = emailClient.GetMessage(i);
                var emailMessage = new EmailMessage
                {
                    Content = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                    Subject = message.Subject,
                    SenderName = message.Sender.Name,
                    SenderEmail = message.Sender.Address
                };
                emails.Add(emailMessage);
            }

            return emails;
        }
    }

    public class EmailMessage
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiveName { get; set; }
        public string ReceiveEmail { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}