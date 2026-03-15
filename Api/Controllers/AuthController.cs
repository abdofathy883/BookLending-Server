using Application.DTOs.Auth;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result.IsAuthenticated)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDTO request)
        {
            if (request is null)
                return BadRequest("بيانات المستخدم غير صحيحة");
            try
            {
                var result = await authService.RegisterUserAsync(request);
                if (result.IsAuthenticated)
                    return Ok(result);
                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await authService.GetAllUsersAsync();
            if (users.Count > 0)
            {
                return Ok(users);
            }
            return NotFound("لا يوجد مستخدمين");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var user = await authService.GetUserByIdAsync(userId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("المستخدم غير موجود");
        }
    }
}
