namespace Services.Services.CacheService
{
    public interface ICacheService
    {
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCacheResponseAsync(string cacheKey);
    }
}
