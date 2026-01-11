using Identity.Data;
using Identity.Repository;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Services
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshToken(string userId, int days = 7);
        string HashToken(string rawToken);
        Task<bool> ValidateRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string rawToken);
        Task RevokeAllUserRefreshTokens(string userId);
        Task RevokeRefreshToken(RefreshToken refreshToken, string? replacedByTokenHash = null);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> GenerateRefreshToken(string userId, int days = 7)
        {
            var rawToken = GenerateSecureTokenBase64Url();
            var tokenHash = HashToken(rawToken);

            await _refreshTokenRepository.AddRefreshTokenAsync(
                userId: userId,
                tokenHash: tokenHash,
                expiresAt: DateTime.UtcNow.AddDays(days));

            return rawToken;
        }

        private static string GenerateSecureTokenBase64Url(int bytes = 64)
        {
            var buffer = RandomNumberGenerator.GetBytes(bytes);
            return WebEncoders.Base64UrlEncode(buffer);
        }

        public string HashToken(string rawToken)
        {
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }

        public Task<bool> ValidateRefreshToken(RefreshToken refreshToken)
        {
            var isValid = refreshToken.RevokedAt == null && refreshToken.ExpiresAt > DateTime.UtcNow;
            return Task.FromResult(isValid);
        }

        public Task<RefreshToken?> GetRefreshTokenAsync(string rawToken)
        {
            var hash = HashToken(rawToken);
            return _refreshTokenRepository.GetRefreshTokenByHashAsync(hash);
        }

        public async Task RevokeAllUserRefreshTokens(string userId)
        {
            await _refreshTokenRepository.RevokeAllByUserIdAsync(userId);
        }

        public async Task RevokeRefreshToken(RefreshToken refreshToken, string? replacedByTokenHash = null)
        {
            await _refreshTokenRepository.RevokeByIdAsync(refreshToken.Id, replacedByTokenHash);
        }
    }
}
