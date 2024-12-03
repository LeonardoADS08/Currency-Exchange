using CurrencyExchange.Core.Models.Response;

namespace CurrencyExchange.Core.Services.Interfaces;

public interface IExchangeRateService
{
    Task<ExchangeRateDto> GetExchangeRates(string baseCurrency, string[] targetCurrencies);
    Task<ICollection<CurrencyDto>> GetCurrencies();
}