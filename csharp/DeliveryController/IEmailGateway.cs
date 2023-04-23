namespace DeliveryController;

public interface IEmailGateway
{
    void Send(string address, string subject, string message);
}