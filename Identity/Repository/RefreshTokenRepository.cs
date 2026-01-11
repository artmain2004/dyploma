using Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Repository
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(string userId, string tokenHash, DateTime expiresAt);
        Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash);
        Task RevokeAllByUserIdAsync(string userId);
        Task RevokeByIdAsync(Guid refreshTokenId, string? replacedByTokenHash = null);
    }

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _context;

        public RefreshTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(string userId, string tokenHash, DateTime expiresAt)
        {
            var entity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = tokenHash,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                RevokedAt = null,
                ReplacedByTokenHash = null
            };

            await _context.RefreshTokens.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash)
        {
            return _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task RevokeAllByUserIdAsync(string userId)
        {
            await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rt => rt.RevokedAt, DateTime.UtcNow));
        }

        public async Task RevokeByIdAsync(Guid refreshTokenId, string? replacedByTokenHash = null)
        {
            await _context.RefreshTokens
                .Where(rt => rt.Id == refreshTokenId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rt => rt.RevokedAt, DateTime.UtcNow)
                    .SetProperty(rt => rt.ReplacedByTokenHash, replacedByTokenHash));
        }
    }
}
