using AuthMicroservice.Contracts;
using AuthMicroservice.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProductsMicroService.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace AuthMicroservice.Services
{
    public class AuthService : IAuth
    {
        private readonly AuthContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ITokenRevocation _revocationService;
        public AuthService(AuthContext context,IConfiguration configuration, ITokenRevocation revocationService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _passwordHasher = new PasswordHasher<User>();
            _revocationService = revocationService;
        }
        public async Task<string> LoginAsync(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return null;

            var user = _context.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            var token = GenerateJwtToken(user.UserName);
            return token;
        }

        public async Task LogoutAsync(string token)
        {
            //Revoke the token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Revoke token
            await _revocationService.RevokeTokenAsync(token, jwtToken.ValidTo);
        }

        public Task<string> RefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"GENERATED TOKEN: {tokenString}");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RegisterUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

    }
    
}
