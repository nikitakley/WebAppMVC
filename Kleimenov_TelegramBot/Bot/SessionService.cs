using Kleimenov_TelegramBot.Dto;
using System.Collections.Concurrent;

public class SessionService
{
    private readonly ConcurrentDictionary<long, UserSession> _sessions = new();

    public void SetSession(long chatId, UserSession session)
    {
        _sessions[chatId] = session;
    }

    public UserSession? GetSession(long chatId)
    {
        return _sessions.GetValueOrDefault(chatId);
    }

    public void RemoveSession(long chatId)
    {
        _sessions.TryRemove(chatId, out _);
    }
}

public class CartItem
{
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
}

//public record UserSession(string Token, CustomerDto Customer);
public class UserSession
{
    public string Token { get; }
    public CustomerDto Customer { get; }
    public List<CartItem> CartItems { get; } = new();

    public UserSession(string token, CustomerDto customer)
    {
        Token = token;
        Customer = customer;
    }
}