using Application.DTOs.Auth;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly BookLendingDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJWTServices jwtServices;
        public AuthService(
            BookLendingDbContext _context,
            UserManager<ApplicationUser> _userManager,
            IJWTServices _jwtServices
            )
        {
            dbContext = _context;
            userManager = _userManager;
            jwtServices = _jwtServices;
        }
        public async Task<List<AuthResponseDTO>> GetAllUsersAsync()
        {
            var users = await userManager.Users.ToListAsync()
                ?? throw new KeyNotFoundException("لا يوجد مستخدمين");

            var result = new List<AuthResponseDTO>();

            foreach (var u in users)
            {
                var roles = await userManager.GetRolesAsync(u);
                result.Add(new AuthResponseDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                    Roles = roles.ToList()
                });
            }
            return result;
        }
        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var user = await GetUserOrThrow(userId);
            var roles = await userManager.GetRolesAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList()
            };
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO login)
        {
            var authDTO = new AuthResponseDTO();
            var user = await userManager.FindByEmailAsync(login.Email.Trim());
            var passCheck = await userManager.CheckPasswordAsync(user, login.Password.Trim());
            if (user is null || !passCheck)
            {
                authDTO.IsAuthenticated = false;
                authDTO.Message = "لا يوجد حساب بهذه البيانات";
                return authDTO;
            }

            var roles = await userManager.GetRolesAsync(user);
            authDTO.Id = user.Id;
            authDTO.FullName = user.FullName;
            authDTO.Email = user.Email ?? string.Empty;
            authDTO.UserName = user.UserName;
            authDTO.PhoneNumber = user.PhoneNumber ?? string.Empty;
            authDTO.Roles = roles.ToList();
            authDTO.IsAuthenticated = true;

            var tokenPayload = new TokenUserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = await userManager.GetRolesAsync(user),
                Claims = await userManager.GetClaimsAsync(user),
            };
            authDTO.Token = await jwtServices.GenerateAccessTokenAsync(tokenPayload);


            if (user.RefreshTokens.Any(u => u.IsActive))
            {
                var ActiveRefreshToken = user.RefreshTokens.First(t => t.IsActive);
                authDTO.RefreshToken = ActiveRefreshToken.Token;
                authDTO.RefreshTokenExpiration = ActiveRefreshToken.ExpiresOn;
            }
            else
            {
                var RefreshToken = await jwtServices.GenerateRefreshTokenAsync();
                authDTO.RefreshToken = RefreshToken.Token;
                authDTO.RefreshTokenExpiration = RefreshToken.ExpiresOn;
                user.RefreshTokens.Add(RefreshToken);
                await userManager.UpdateAsync(user);
            }

            authDTO.Message = "تم تسجيل الدخول بنجاح";
            return authDTO;
        }
        public async Task<AuthResponseDTO> RegisterUserAsync(RegisterUserDTO newUser)
        {
            var validateErrors = await ValidateRegisterAsync(newUser);
            if (validateErrors is not null && validateErrors.Count > 0)
                return FailResult(string.Join(", ", validateErrors));

            var user = new ApplicationUser
            {
                FullName = newUser.FullName,
                UserName = newUser.Email.Split("@")[0],
                Email = newUser.Email,
                EmailConfirmed = true,
                PhoneNumber = newUser.PhoneNumber,
                PhoneNumberConfirmed = true
            };

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await userManager.CreateAsync(user, newUser.Password);

                if (!result.Succeeded)
                    return FailResult("Failed To Add New User");

                await userManager.AddToRoleAsync(user, newUser.Role.ToString());

                var authDTO = new AuthResponseDTO
                {
                    IsAuthenticated = true,
                    Message = "تم تسجيل حساب جديد بنجاح"
                };

                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return authDTO;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
        public static AuthResponseDTO FailResult(string message)
        {
            return new AuthResponseDTO
            {
                IsAuthenticated = false,
                Message = message
            };
        }
        public async Task<List<string>> ValidateRegisterAsync(RegisterUserDTO registerDTO)
        {
            var errors = new List<string>();

            // Email Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                errors.Add("بريد الكتروني غير صالح");
            }
            if (await userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                errors.Add("هذا الايميل موجود بالفعل");
            }

            //Password Validation
            if (string.IsNullOrWhiteSpace(registerDTO.Password))
            {
                errors.Add("الرقم السري مطلوب");
            }
            else if (registerDTO.Password.Length < 6)
            {
                errors.Add("الرقم السري يجب ان يكون 6 احرف على الاقل");
            }

            //Phone 
            if (string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
            {
                errors.Add("رقم الهاتف مطلوب");
            }

            //************** Phone Unique Validation
            if (await userManager.Users.AnyAsync(u => u.PhoneNumber == registerDTO.PhoneNumber))
            {
                errors.Add("رقم الهاتف موجود بالفعل");
            }

            //Name
            if (string.IsNullOrWhiteSpace(registerDTO.FullName))
            {
                errors.Add("الاسم مطلوب");
            }

            return errors;
        }
        private async Task<ApplicationUser> GetUserOrThrow(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("لم يتم العثور على المستخدم");
            return user;
        }
    }
}