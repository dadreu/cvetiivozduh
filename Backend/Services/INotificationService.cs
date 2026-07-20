namespace FlowerShop.Backend.Services;

public interface INotificationService
{
    Task SendOrderNotificationAsync(string message);
}
