using AuthMicroservice.Contracts;
using AuthMicroservice.Models;
using AuthMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthMicroservice.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IAuth _authService;
        public AuthController(IConfiguration configuration, IAuth authService)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid login request.");
            }
            var token = await _authService.LoginAsync(model);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(new { Token = token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token.");
            }
            await _authService.LogoutAsync(token);
            return Ok("Logged out successfully.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Invalid registration request.");
            }
            try
            {
                await _authService.RegisterUser(user);
                return Ok("User registered successfully.");
            }
            catch (DbUpdateException)
            {
                return Conflict("Username or email already exists.");
            }
        }


        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            // Extract token from Authorization header
            var token = HttpContext.Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            await _authService.LogoutAsync(token);
            return Ok(new { message = "Token revoked successfully" });
        }

    }
}
