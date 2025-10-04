using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using FossTech.Helpers;

namespace FossTech.Helpers
{
    public interface IEmailService
    {
        void Send(EmailService.EmailMessage emailMessage);
    }
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public void Send(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(emailMessage.ToAddress.Name, emailMessage.ToAddress.Address));
            message.From.Add(new MailboxAddress(emailMessage.FromAddress.Name, emailMessage.FromAddress.Address));

            message.Subject = emailMessage.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using var emailClient = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };

            emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);

            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

            emailClient.Send(message);

            emailClient.Disconnect(true);
        }

        public class EmailMessage
        {
            public EmailAddress ToAddress { get; set; }
            public EmailAddress FromAddress { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
        }

        public class EmailAddress
        {
            public string Name { get; set; }
            public string Address { get; set; }

        }
    }
}
