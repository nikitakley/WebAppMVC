using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Kleimenov_TelegramBot.Dto;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseAddress;

    public ApiService(string baseAddress)
    {
        _baseAddress = baseAddress.TrimEnd('/');
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseAddress)
        };

        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    }

    public async Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var request = new
        {
            username,
            password
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/login", content, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Неверный логин или пароль");
        }
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseJson, options);

        if (loginResponse?.Token == null || loginResponse.Customer == null)
            throw new InvalidOperationException("Некорректный ответ от сервера");

        return loginResponse;
    }

    public async Task<List<RestaurantDto>> GetRestaurantsAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync("/api/restaurants", ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);    // читает тело ответа как строку json
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };     // несовпадение имен свойств по регистру
        var restaurants = JsonSerializer.Deserialize<List<RestaurantDto>>(json, options);

        return restaurants ?? new List<RestaurantDto>();
    }

    public async Task<MenuDto?> GetMenuAsync(int restaurantId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/restaurants/{restaurantId}/menu", ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var menu = JsonSerializer.Deserialize<MenuDto>(json, options);

        return menu;
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int customerId, string token, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customers/{customerId}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<CustomerDto>(json, options)
               ?? throw new InvalidOperationException("Некорректный ответ от сервера");
    }

    public async Task<List<OrderDtoCustomer>> GetOrdersByCustomerAsync(int customerId, string token, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/orders/customer/{customerId}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var orders = JsonSerializer.Deserialize<List<OrderDtoCustomer>>(json, options);
        return orders ?? new List<OrderDtoCustomer>();
    }

    public async Task<OrderDtoCustomer?> GetOrderByIdAsync(int orderId, int customerId, string token, CancellationToken ct = default)
    {
        var orders = await GetOrdersByCustomerAsync(customerId, token, ct);
        return orders.FirstOrDefault(o => o.OrderId == orderId);
    }

    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderDto, string token, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(orderDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<OrderResponseDto>(responseJson, options)
               ?? throw new InvalidOperationException("Некорректный ответ от сервера");
    }

    public async Task CreateOrderItemAsync(OrderItemRequestDto itemDto, string token, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(itemDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/items")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}