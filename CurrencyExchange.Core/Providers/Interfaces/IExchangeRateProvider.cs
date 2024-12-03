using CurrencyExchange.Core.Models;

namespace CurrencyExchange.Core.Providers.Interfaces;

public interface IExchangeRateProvider
{
    Task<ExchangeRate> GetExchangeRates(string baseCurrency, string[] targetCurrencies);
    Task<ICollection<Currency>> GetCurrencies();
}