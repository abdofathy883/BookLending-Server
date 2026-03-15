using Application.DTOs.Auth;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IJWTServices
    {
        Task<string> GenerateAccessTokenAsync(TokenUserDto user);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}
