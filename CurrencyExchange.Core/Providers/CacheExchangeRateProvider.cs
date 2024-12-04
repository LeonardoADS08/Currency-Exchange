using CurrencyExchange.Core.Models;
using CurrencyExchange.Core.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CurrencyExchange.Core.Providers;

internal class CacheExchangeRateProvider : IExchangeRateProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly IExchangeRateProvider _decoratedProvider;
    private readonly ILogger<CacheExchangeRateProvider> _logger;

    // TODO: make this configurable
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

    public CacheExchangeRateProvider(IMemoryCache memoryCache,
        IExchangeRateProvider decoratedProvider,
        ILogger<CacheExchangeRateProvider> logger)
    {
        _memoryCache = memoryCache;
        _decoratedProvider = decoratedProvider;
        _logger = logger;
    }

    public async Task<ExchangeRate> GetExchangeRates(string baseCurrency, string[] targetCurrencies)
    {
        var cacheKey = GetExchangeRateCacheKey(baseCurrency, targetCurrencies);

        if (!_memoryCache.TryGetValue(cacheKey, out ExchangeRate? exchangeRate))
        {
            _logger.LogInformation("Cache miss for {CacheKey}. Fetching from decorated provider.", cacheKey);
            exchangeRate = await _decoratedProvider.GetExchangeRates(baseCurrency, targetCurrencies);
            _logger.LogInformation("Successfully fetched exchange rates for {CacheKey} from decorated provider: {Provider}.", cacheKey, exchangeRate.Provider);
            _memoryCache.Set(cacheKey, exchangeRate, _cacheDuration);
        }

        return exchangeRate!;
    }

    public async Task<ICollection<Currency>> GetCurrencies()
    {
        const string cacheKey = "currencies";

        if (!_memoryCache.TryGetValue(cacheKey, out ICollection<Currency>? currencies))
        {
            _logger.LogInformation("Cache miss for {CacheKey}. Fetching from decorated provider.", cacheKey);
            currencies = await _decoratedProvider.GetCurrencies();
            _logger.LogInformation("Successfully fetched currencies for {CacheKey} from decorated provider.", cacheKey);
            _memoryCache.Set(cacheKey, currencies, _cacheDuration);
        }

        return currencies!;
    }

    private string GetExchangeRateCacheKey(string baseCurrency, string[] targetCurrencies)
    {
        var targetCurrenciesString = string.Join(",", targetCurrencies);
        return $"{baseCurrency}:{targetCurrenciesString}";
    }
}