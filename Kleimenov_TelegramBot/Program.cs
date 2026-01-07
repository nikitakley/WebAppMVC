using Telegram.Bot;

class Program
{
    static async Task Main()
    {
        var botToken = "8203125426:AAGZmRcLNxRb-PUmt9-15LdSmfdwmadVeqg";
        var botClient = new TelegramBotClient(botToken);

        var apiBaseUrl = "https://localhost:7112";
        var apiService = new ApiService(apiBaseUrl);

        var sessionService = new SessionService();

        var botService = new BotService(botClient, apiService, sessionService);
        await botService.StartAsync();

        Console.ReadLine();
    }
}