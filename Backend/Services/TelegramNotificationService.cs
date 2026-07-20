namespace FlowerShop.Backend.Services;

public class TelegramNotificationService : INotificationService
{
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(ILogger<TelegramNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendOrderNotificationAsync(string message)
    {
        // Временно используем логгер как заглушку, пока нет токена Telegram
        // Здесь будет код вида: await httpClient.PostAsync($"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}&text={message}");
        _logger.LogInformation("================ NEW ORDER ================");
        _logger.LogInformation(message);
        _logger.LogInformation("===========================================");
        
        return Task.CompletedTask;
    }
}
