using Microsoft.Extensions.Caching.Distributed;
using ProductsMicroService.Contracts;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace ProductsMicroService.Services
{
    public class RedisTokenRevocationService : ITokenRevocation
    {
        private readonly IDatabase _db;

        public RedisTokenRevocationService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task RevokeTokenAsync(string token, DateTime expiration)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var jti = jwt.Id;

            var timeToExpire = expiration.ToUniversalTime() - DateTime.UtcNow;
            if (timeToExpire <= TimeSpan.Zero) return;

            var key = $"revoked_token:{jti}";
            await _db.StringSetAsync(key, "revoked", timeToExpire);
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return false;

            var jwt = handler.ReadJwtToken(token);
            var jti = jwt.Id;
            var key = $"revoked_token:{jti}";

            return await _db.KeyExistsAsync(key);
        }
    }

}
