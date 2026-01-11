using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Identity.Middleware
{
    public sealed class ApiKeyMiddleware : IMiddleware
    {
        private const string HeaderName = "X-API-Key";

        private readonly IdentityDbContext _context;
        private readonly IMemoryCache _cache;

        public ApiKeyMiddleware(IdentityDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        { 
            if (!context.Request.Headers.TryGetValue(HeaderName, out StringValues values) ||
                StringValues.IsNullOrEmpty(values))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key is missing");
                return;
            }

            var apiKey = values.Count == 1 ? values[0] : null;
            apiKey = apiKey?.Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key is missing");
                return;
            }

            if (values.Count > 1)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API key header");
                return;
            }

            var isValid = await IsValidApiKeyAsync(apiKey, context.RequestAborted);
            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            context.Items["api_key"] = apiKey;

            await next(context);
        }

        private Task<bool> IsValidApiKeyAsync(string apiKey, CancellationToken ct)
        {
            var cacheKey = $"apikey_valid::{apiKey}";

            return _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                return await _context.ApiKeys
                    .AsNoTracking()
                    .AnyAsync(k => k.Key == apiKey && k.IsValid, ct);
            });
        }

    }
}
