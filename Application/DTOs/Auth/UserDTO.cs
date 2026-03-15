namespace Application.DTOs.Auth
{
    public class UserDTO
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public List<string>? Roles { get; set; }
    }
}
