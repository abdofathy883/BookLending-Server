using Application.Interfaces;
using Application.MappingProfiles;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Settings;
using Hangfire;
using Infrastructure.Identity;
using Infrastructure.Persistance;
using Infrastructure.Services.Books;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<BookLendingDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BookLendingDbContext>()
            .AddDefaultTokenProviders();

            JWTSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JWTSettings>()
                ?? throw new Exception("Error in JWT Settings");

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSetting"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier,
                        ValidateLifetime = true,
                    };
                });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJWTServices, JWTService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookBorrowService, BookBorrowService>();
            builder.Services.AddScoped<OverdueBooksJob>();

            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

            builder.Services.AddHangfireServer();

            builder.Services.AddAutoMapper(cgh =>
            {
                cgh.AddProfile<BookProfile>();
                cgh.AddProfile<BookBorrowProfile>();
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Prod", policy =>
                {
                    policy.WithOrigins("http://livedomain.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Dev", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });


            var app = builder.Build();

            app.UseHangfireDashboard("/hangfire");

            RecurringJob.AddOrUpdate<OverdueBooksJob>(
                recurringJobId: "check-overdue-books",
                methodCall: job => job.CheckOverdueBooksAsync(),
                cronExpression: Cron.Daily,
                options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
            );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseCors("Dev");
            }

            app.UseHttpsRedirection();

            app.UseCors("Prod");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<BookLendingDbContext>();
                await dbContext.Database.MigrateAsync();
                await AuthSeeder.SeedAsync(services);
            }

            app.Run();
        }
    }
}
