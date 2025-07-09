using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryService.Infrastructure.Services;
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    public TokenService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    public async Task<string> GenerateTokenAsync(string username, string role, int userId)
    {
        var secretKey = _jwtSettings.SecretKey;
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: new[] { new Claim(ClaimTypes.Name, "admin") },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return await Task.FromResult(tokenString); 
    }
}
