using CurrencyExchange.Core.Providers;
using CurrencyExchange.Core.Providers.FX;
using CurrencyExchange.Core.Providers.Interfaces;
using CurrencyExchange.Core.Services;
using CurrencyExchange.Core.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CurrencyExchange.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCurrencyExchangeService(this IServiceCollection services)
    {
        services.AddHttpClient<FXRatesProvider>();

        services.AddMemoryCache();
        services.AddSingleton<IExchangeRateProvider>(sp =>
        {
            var decoratedProvider = sp.GetRequiredService<FXRatesProvider>();
            var cache = sp.GetRequiredService<IMemoryCache>();
            var logger = sp.GetRequiredService<ILogger<CacheExchangeRateProvider>>();
            return new CacheExchangeRateProvider(cache, decoratedProvider, logger);
        });

        services.AddSingleton<IExchangeRateService, ExchangeRateService>();

        return services;
    }
}