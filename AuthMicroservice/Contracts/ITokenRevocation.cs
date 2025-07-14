namespace ProductsMicroService.Contracts
{
    public interface ITokenRevocation
    {
        Task RevokeTokenAsync(string token, DateTime expiration);
        Task<bool> IsTokenRevokedAsync(string token);
    }
}
