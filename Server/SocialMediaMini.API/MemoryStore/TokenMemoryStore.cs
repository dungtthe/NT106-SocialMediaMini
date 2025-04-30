using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace SocialMediaMini.API.MemoryStore
{
    public class TokenMemoryStore
    {
        private readonly IMemoryCache _cache;
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();

        public TokenMemoryStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        private SemaphoreSlim GetLock(int userId)
        {
            return _locks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
        }

        public async Task AddTokenAsync(int userId, string token)
        {
            var semaphore = GetLock(userId);
            await semaphore.WaitAsync();
            try
            {
                _cache.Set(userId, token);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task RevokeTokenAsync(int userId)
        {
            var semaphore = GetLock(userId);
            await semaphore.WaitAsync();
            try
            {
                _cache.Remove(userId);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> IsTokenValidAsync(string token, int userId)
        {
            var semaphore = GetLock(userId);
            await semaphore.WaitAsync();
            try
            {
                return _cache.TryGetValue(userId, out string cachedToken) && cachedToken == token;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<string> GetTokenByUserIdAsync(int userId)
        {
            var semaphore = GetLock(userId);
            await semaphore.WaitAsync();
            try
            {
                return _cache.TryGetValue(userId, out string cachedToken) ? cachedToken : null;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

}
