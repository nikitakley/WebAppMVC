using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

public class BotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    public BotService(ITelegramBotClient botClient, ApiService apiService, SessionService sessionService)
    {
        _botClient = botClient;
        _apiService = apiService;
        _sessionService = sessionService;
    }

    public async Task StartAsync()
    {
        using var cts = new CancellationTokenSource();

        // настройка типов получаемых обновлений
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        // long polling - бесконечный цикл, слушаем сообщения
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,    // вызывающийся каждый раз при очередном сообщении метод
            pollingErrorHandler: HandleErrorAsync,    // метод при ошибках
            receiverOptions: receiverOptions,    // настройка фильтрации сообщений
            cancellationToken: cts.Token    // токен отмены для остановки
        );

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Бот @{me.Username} запущен!");
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Telegram.Bot.Types.Update update,
        CancellationToken cancellationToken)
    {
        var handler = new UpdateHandler(botClient, _apiService, _sessionService);
        await handler.HandleAsync(update, cancellationToken);
    }

    private Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
}
