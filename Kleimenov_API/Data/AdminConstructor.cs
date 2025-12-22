using Kleimenov_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Data;

public static class AdminConstructor
{
    public static async Task InitializeAdminAsync(Kleimenov_APIContext context)
    {
        // Проверяем, есть ли уже админ
        if (await context.Users.AnyAsync(u => u.Role == "Admin"))
            return;

        // Создаём админ-клиента
        var adminCustomer = new Customer
        {
            FullName = "Администратор",
            Phone = "+70000000000",
            Email = "admin@gmail.com",
            Address = "",
            RegisteredAt = DateTime.UtcNow
        };

        context.Customers.Add(adminCustomer);
        await context.SaveChangesAsync();

        // Хешируем пароль
        var hasher = new PasswordHasher<User>();
        var passwordHash = hasher.HashPassword(null, "admin123");

        // Создаём админ-пользователя
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = passwordHash,
            Role = "Admin",
            CustomerId = adminCustomer.CustomerId
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
    }
}
