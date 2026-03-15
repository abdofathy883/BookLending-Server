using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public List<BookBorrow> BookBorrows { get; set; } = new();
        public List<OverdueBorrow> OverdueBorrows { get; set; } = new();
    }
}
