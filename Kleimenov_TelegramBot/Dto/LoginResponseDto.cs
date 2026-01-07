namespace Kleimenov_TelegramBot.Dto;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public CustomerDto Customer { get; set; } = new();
}