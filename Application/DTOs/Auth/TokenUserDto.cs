using System.Security.Claims;

namespace Application.DTOs.Auth
{
    public class TokenUserDto
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = [];
        public IList<Claim> Claims { get; set; } = [];
    }
}
