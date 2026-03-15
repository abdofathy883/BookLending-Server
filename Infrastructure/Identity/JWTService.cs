using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity
{
    public class JWTService : IJWTServices
    {
        private readonly IConfiguration configuration;
        public JWTService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async Task<string> GenerateAccessTokenAsync(TokenUserDto user)
        {
            var key = configuration["JWT:Key"];
            var issuer = configuration["JWT:Issuer"];
            var audience = configuration["JWT:Audience"];
            var expiration = int.Parse(configuration["JWT:ExpirationMinutes"]!);
            var roleClaims = user.Roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Sub, user.Id),
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                new (JwtRegisteredClaimNames.Email, user.Email ?? "")
            }.Union(user.Claims)
                 .Union(roleClaims);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiration),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return accessToken;
        }

        public Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            var refreshTokenExpiration = int.Parse(configuration["JWT:RefreshTokenExpirationDays"]!);
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Task.FromResult(new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                CreateOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpiration)
            });
        }
    }
}
