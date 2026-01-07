using Kleimenov_TelegramBot.Dto;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class UpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    public UpdateHandler(ITelegramBotClient botClient, ApiService apiService, SessionService sessionService)
    {
        _botClient = botClient;
        _apiService = apiService;
        _sessionService = sessionService;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        if (update?.Message?.Text is not { } text)
            return;

        await HandleMessageAsync(update.Message, text, cancellationToken);
    }

    private async Task HandleMessageAsync(
        Message message,
        string text,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        if (text.StartsWith("/login ", StringComparison.OrdinalIgnoreCase))
        {
            var args = text["/login ".Length..].Trim();
            var parts = args.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Используйте: /login [логин] [пароль]\nПример: /login ivan 123456",
                    cancellationToken: cancellationToken);
                return;
            }

            var username = parts[0];
            var password = parts[1];

            await SendLoginAsync(chatId, username, password, cancellationToken);
            return;
        }

        if (text.StartsWith("/menu ", StringComparison.OrdinalIgnoreCase))
        {
            var idr = text["/menu ".Length..].Trim();

            if (int.TryParse(idr, out var restaurantId) && restaurantId > 0)
            {
                await SendMenuAsync(chatId, restaurantId, cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    chatId, "Неверный формат. Корректный пример команды: /menu 1", cancellationToken: cancellationToken);
            }
            return;
        }

        if (text.StartsWith("/add ", StringComparison.OrdinalIgnoreCase))
        {
            var session = _sessionService.GetSession(chatId);
            if (session == null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId, 
                    "Сначала войдите в аккаунт: /login [логин] [пароль]", 
                    cancellationToken: cancellationToken
                );
                return;
            }

            var parts = text["/add ".Length..].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3 ||
                !int.TryParse(parts[0], out var restaurantId) ||
                !int.TryParse(parts[1], out var dishId) ||
                !int.TryParse(parts[2], out var quantity) ||
                quantity <= 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Неверный формат. Корректный пример команды: /add 1 5 2",
                    cancellationToken: cancellationToken);
                return;
            }

            await SendAddItemCartAsync(chatId, session, restaurantId, dishId, quantity, cancellationToken);
            return;
        }

        if (text.StartsWith("/remove ", StringComparison.OrdinalIgnoreCase))
        {
            var session = _sessionService.GetSession(chatId);
            if (session == null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Сначала войдите в аккаунт: /login [логин] [пароль]", 
                    cancellationToken: cancellationToken);
                return;
            }

            if (session.CartItems.Count == 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Корзина пуста.",
                    cancellationToken: cancellationToken);
                return;
            }

            var indexStr = text["/remove ".Length..].Trim();
            if (!int.TryParse(indexStr, out var index) || index < 1 || index > session.CartItems.Count)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    $"Укажите номер от 1 до {session.CartItems.Count}",
                    cancellationToken: cancellationToken);
                return;
            }

            var removed = session.CartItems[index - 1];
            session.CartItems.RemoveAt(index - 1);

            await _botClient.SendTextMessageAsync(
                chatId,
                $"🗑️ Удалено: {removed.DishName}",
                cancellationToken: cancellationToken);
            return;
        }

        switch (text)
        {
            case "/start":
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text:
                        "Добро пожаловать!\n\nСписок команд:\n" +
                        "📍 /restaurants — список ресторанов\n" +
                        "📋 /menu [ID] — меню выбранного ресторана\n\n" +
                        "🔑 /login [логин] [пароль] — войти в аккаунт\n" +
                        "👤 /profile — мой профиль\n" +
                        "📦 /orders — мои заказы\n\n" +
                        "🛒 /cart — моя корзина\n" +
                        "✚ /add [restID] [dishID] [quant] — добавить блюдо в корзину\n" +
                        "▬ /remove [position] — удалить позицию в корзине\n" +
                        "🧹 /clearcart — очистить корзину\n" +
                        "🚀 /checkout — оформить заказ\n",
                    cancellationToken: cancellationToken
                );
                break;

            case "/restaurants":
                await SendRestaurantsListAsync(chatId, cancellationToken);
                break;

            case "/profile":
                var profileSession = _sessionService.GetSession(chatId);
                if (profileSession == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "Сначала войдите в аккаунт: /login [логин] [пароль]",
                        cancellationToken: cancellationToken);
                    return;
                }

                await SendProfileAsync(chatId, profileSession, cancellationToken);
                break;

            case "/orders":
                var orderSession = _sessionService.GetSession(chatId);
                if (orderSession == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "Сначала войдите в аккаунт: /login [логин] [пароль]",
                        cancellationToken: cancellationToken);
                    return;
                }

                await SendOrdersAsync(chatId, orderSession, cancellationToken);
                break;

            case "/cart":
                var cartSession = _sessionService.GetSession(chatId);
                if (cartSession == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "Сначала войдите в аккаунт: /login [логин] [пароль]", 
                        cancellationToken: cancellationToken);
                    return;
                }

                await SendCartAsync(chatId, cartSession, cancellationToken);
                break;

            case "/clearcart":
                var clearSession = _sessionService.GetSession(chatId);
                if (clearSession == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "Сначала войдите в аккаунт: /login [логин] [пароль]",
                        cancellationToken: cancellationToken);
                    return;
                }
                
                clearSession.CartItems.Clear();
                await _botClient.SendTextMessageAsync(chatId, "Корзина очищена.", cancellationToken: cancellationToken);
                break;

            case "/checkout":
                var checkoutSession = _sessionService.GetSession(chatId);
                if (checkoutSession == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId, 
                        "Сначала войдите в аккаунт: /login [логин] [пароль]", 
                        cancellationToken: cancellationToken);
                    return;
                }

                await SendCheckoutAsync(chatId, checkoutSession, cancellationToken);
                break;

            default:
                await _botClient.SendTextMessageAsync(
                    message.Chat.Id, "Неизвестная команда", cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task SendLoginAsync(long chatId, string username, string password, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _apiService.LoginAsync(username, password, cancellationToken);
            _sessionService.SetSession(chatId, new UserSession(response.Token, response.Customer));

            await _botClient.SendTextMessageAsync(
                chatId,
                $"✅ Успешный вход!\nЗдравствуйте, {response.Customer.FullName}!",
                cancellationToken: cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Неверный логин или пароль. Попробуйте снова.",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка входа: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Не удалось войти. Проверьте подключение к серверу.",
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendRestaurantsListAsync(long chatId, CancellationToken cancellationToken)
    {
        try
        {
            var restaurants = await _apiService.GetRestaurantsAsync(cancellationToken);

            if (restaurants.Count == 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId, "Нет доступных ресторанов", cancellationToken: cancellationToken);
                return;
            }

            var messageText = "Cписок ресторанов:\n\n";
            foreach (var r in restaurants)
            {
                messageText += $"🏬 {r.Name} | ID: {r.RestaurantId} \nРейтинг: ⭐{r.Rating}\n\n";
            }

            await _botClient.SendTextMessageAsync(
                chatId, messageText, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении ресторанов: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId, "❌ Не удалось загрузить рестораны. Попробуйте позже.", cancellationToken: cancellationToken);
        }
    }

    private async Task SendMenuAsync(long chatId, int restaurantId, CancellationToken cancellationToken)
    {
        try
        {
            var menu = await _apiService.GetMenuAsync(restaurantId, cancellationToken);

            if (menu == null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId, "Ресторан не найден.", cancellationToken: cancellationToken);
                return;
            }

            if (menu.Dishes.Count == 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId, $"📋 Меню ресторана {menu.Name} [ID: {menu.RestaurantId}]:\n\nПусто.", cancellationToken: cancellationToken);
                return;
            }

            var messageText = $"📋 Меню ресторана {menu.Name} [ID: {menu.RestaurantId}]:\n\n";
            foreach (var dish in menu.Dishes)
            {
                if (!dish.IsAvailable)
                    continue;

                messageText += $"•  {dish.Name} [id: {dish.DishId}] — {dish.Price:C}\n";
            }

            await _botClient.SendTextMessageAsync(
                chatId, messageText, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении меню: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId, "❌ Не удалось загрузить меню. Попробуйте позже.", cancellationToken: cancellationToken);
        }
    }

    private async Task SendAddItemCartAsync(long chatId, UserSession session, int restaurantId, int dishId, int quantity, CancellationToken cancellationToken)
    {
        try
        {
            var menu = await _apiService.GetMenuAsync(restaurantId, cancellationToken);
            if (menu == null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Ресторан не найден.",
                    cancellationToken: cancellationToken);
                return;
            }

            var dish = menu.Dishes.FirstOrDefault(d => d.DishId == dishId);
            if (dish == null || !dish.IsAvailable)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Блюдо не найдено или недоступно.",
                    cancellationToken: cancellationToken);
                return;
            }

            if (session.CartItems.Count > 0)
            {
                var existingRestaurantId = session.CartItems[0].RestaurantId;
                if (existingRestaurantId != restaurantId)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        $"❌ В вашей корзине расположены блюда из другого ресторана \"{session.CartItems[0].RestaurantName}\".\n\n" +
                        "Очистите корзину (/clearcart), чтобы добавить блюда из необходимого ресторана.",
                        cancellationToken: cancellationToken);
                    return;
                }
            }

            var existingItem = session.CartItems.FirstOrDefault(i => i.DishId == dishId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                session.CartItems.Add(new CartItem
                {
                    DishId = dishId,
                    DishName = dish.Name,
                    UnitPrice = dish.Price,
                    Quantity = quantity,
                    RestaurantId = restaurantId,
                    RestaurantName = menu.Name
                });
            }

            await _botClient.SendTextMessageAsync(
                chatId,
                $"✅ Добавлено:\n{dish.Name} × {quantity} = {dish.Price * quantity:F2} ₽",
                cancellationToken: cancellationToken);
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка добавления позиции в корзину: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Не удалось добавить позицию. Возможно, сессия устарела — войдите снова.",
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendCartAsync(long chatId, UserSession session, CancellationToken cancellationToken)
    {
        try
        {
            if (session.CartItems.Count == 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Ваша корзина пуста.",
                    cancellationToken: cancellationToken);
                return;
            }

            var restaurantName = session.CartItems[0].RestaurantName;
            var total = session.CartItems.Sum(i => i.UnitPrice * i.Quantity);

            var message = $"🛒 Корзина\nРесторан: {restaurantName}\n\n";
            for (int i = 0; i < session.CartItems.Count; i++)
            {
                var item = session.CartItems[i];
                message += $"{i + 1}. {item.DishName} — {item.UnitPrice:F2} ₽ × {item.Quantity}\n";
            }
            message += $"\nИтого: {total:F2} ₽\n\n";
            message += "Команды:\n/remove [номер] — удалить позицию\n/clearcart — очистить корзину";

            await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки корзины: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Не удалось загрузить корзину. Возможно, сессия устарела — войдите снова.",
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendProfileAsync(long chatId, UserSession session, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _apiService.GetCustomerByIdAsync(
                session.Customer.CustomerId,
                session.Token,
                cancellationToken);

            var message = $"📋 Информация о профиле:\n\n" +
                          $"🎫 Имя пользователя: {customer.FullName}\n" +
                          $"📞 Телефон: {customer.Phone}\n" +
                          $"✉️ Email: {(string.IsNullOrEmpty(customer.Email) ? "— не указан" : customer.Email)}\n" +
                          $"🏠 Адрес: {customer.Address}\n";
                          //$"Роль: {customer.Role}";

            await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения профиля: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Не удалось загрузить профиль. Возможно, сессия устарела — войдите снова.",
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendOrdersAsync(long chatId, UserSession session, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _apiService.GetOrdersByCustomerAsync(session.Customer.CustomerId, session.Token, cancellationToken);

            if (orders.Count == 0)
            {
                await _botClient.SendTextMessageAsync(chatId, "У вас пока нет заказов.", cancellationToken: cancellationToken);
                return;
            }

            var message = "📦 Ваши заказы:\n\n";
            foreach (var order in orders) // orders.Take(5) - 5 последних
            {
                var total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
                message += $"№{order.OrderId} | {order.RestaurantName}\n";
                message += $"Статус: {order.StatusName}\n";
                message += $"Создан: {order.CreatedAt:dd.MM.yyyy HH:mm}\n";
                message += $"Итого: {total:F2} ₽\n\n";
            }

            await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения заказов: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Не удалось загрузить заказы. Возможно, сессия устарела — войдите снова.",
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendCheckoutAsync(long chatId, UserSession session, CancellationToken cancellationToken)
    {
        try
        {
            if (session.CartItems.Count == 0)
            {
                await _botClient.SendTextMessageAsync(
                    chatId, 
                    "Корзина пуста --> Сначала добавьте блюда.", 
                    cancellationToken: cancellationToken);
                return;
            }

            var restaurantId = session.CartItems[0].RestaurantId;
            var restaurantName = session.CartItems[0].RestaurantName;

            var orderDto = new OrderRequestDto
            {
                CustomerId = session.Customer.CustomerId,
                RestaurantId = restaurantId
            };

            var order = await _apiService.CreateOrderAsync(orderDto, session.Token, cancellationToken);
            var orderId = order.OrderId;

            foreach (var item in session.CartItems)
            {
                var itemDto = new OrderItemRequestDto
                {
                    OrderId = orderId,
                    DishId = item.DishId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                await _apiService.CreateOrderItemAsync(itemDto, session.Token, cancellationToken);
            }

            session.CartItems.Clear();
            await _botClient.SendTextMessageAsync(
                chatId,
                $"✅ Заказ №{orderId} успешно оформлен!\nРесторан: {restaurantName}",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка оформления заказа: {ex}");
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Не удалось оформить заказ. Возможно, сессия устарела — войдите снова.",
                cancellationToken: cancellationToken);
        }
    }
}