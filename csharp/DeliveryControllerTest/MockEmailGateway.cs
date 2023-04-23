using DeliveryController;

namespace DeliveryControllerTest;

public class MockEmailGateway : IEmailGateway
{
    public string EmailMessage { get; set; }
    public bool EmailSent { get; set; }
        
    public void Send(string address, string subject, string message)
    {
        EmailSent = true;
        EmailMessage = message;
    }

    public void Reset()
    {
        EmailSent = false;
        EmailMessage = "";
    }
}