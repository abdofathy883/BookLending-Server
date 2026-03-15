using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity
{
    public static class AuthSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<BookLendingDbContext>();

            // Seed Roles
            var roles = new[] { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed SuperAdmin
            string adminEmail = "abdo.fathy.za@gmail.com";
            string password = "Aa123#";

            var existingUser = await userManager.FindByEmailAsync(adminEmail);
            if (existingUser == null)
            {
                var admin = new ApplicationUser
                {
                    FullName = "Abdelrahman Fathy",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "01028128912",
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    throw new Exception($"Failed to create Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
