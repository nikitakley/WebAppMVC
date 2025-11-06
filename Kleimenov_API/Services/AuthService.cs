using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    public async Task<string?> LoginJWT(string userName, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        if (user == null) 
            return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Unauthorized");

        return GenerateJwtToken(user);
    }

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
