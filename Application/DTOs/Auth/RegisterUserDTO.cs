using Domain.Enums;

namespace Application.DTOs.Auth
{
    public class RegisterUserDTO
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required UserRoles Role { get; set; }
    }
}
