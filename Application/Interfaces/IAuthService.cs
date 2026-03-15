using Application.DTOs.Auth;

namespace Domain.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterUserAsync(RegisterUserDTO newUser);
        Task<AuthResponseDTO> LoginAsync(LoginDTO login);
        Task<List<AuthResponseDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(string userId);
    }
}
