using System.Net;
using System.Net.Mail;

namespace DeliveryController
{
    public interface IEmailGateway
    {
        void send(string address, string subject, string message);
    }

    public class EmailGateway : IEmailGateway
    {
        public void send(string address, string subject, string message)
        {
            var smtpClient = new SmtpClient("localhost")
            {
                Port = 587,
                Credentials = new NetworkCredential("email", "password"),
                EnableSsl = true,
            };
    
            smtpClient.Send("noreply@example.com", address, subject, message);
        }
    }
}