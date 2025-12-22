using Kleimenov_API.Data;
using Kleimenov_API.Dto;
using Kleimenov_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kleimenov_API.Services;

public class AuthService
{
    private readonly Kleimenov_APIContext _context;
    private readonly PasswordHasher<User> _hasher = new();
    private readonly IOptions<AuthSettings> _options;

    public AuthService(Kleimenov_APIContext context, IOptions<AuthSettings> options)
    {
        _context = context;
        _options = options;
    }

    public async Task<bool> RegisterAsync(string Username, string Password, string FullName, string Phone, string Email, string Address)
    {
        if (await _context.Users.AnyAsync(u => u.Username == Username))
            return false;

        var passwordHasher = new PasswordHasher<User>();
        var passwordHash = passwordHasher.HashPassword(null, Password);

        var customer = new Customer
        {
            FullName = FullName,
            Phone = Phone,
            Email = Email,
            Address = Address,
            RegisteredAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var user = new User
        {
            Username = Username,
            PasswordHash = passwordHash,
            Role = "User",
            CustomerId = customer.CustomerId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(string? Token, Customer? Customer, string? Role)> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Username == username);
        if (user?.Customer == null)
            return (null, null, null);

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Unauthorized");

        var token = GenerateJwtToken(user);
        return (token, user.Customer, user.Role);
    }
    
    //public async Task<string?> LoginJWT(string userName, string password)
    //{
    //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
    //    if (user == null) 
    //        return null;

    //    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
    //    if (result == PasswordVerificationResult.Failed)
    //        throw new Exception("Unauthorized");

    //    return GenerateJwtToken(user);
    //}

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(_options.Value.TimeExpires),
            claims: claims,
            signingCredentials:
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.Value.SecretKey)), 
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
