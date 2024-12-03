using CurrencyExchange.Core.Models.Response;
using CurrencyExchange.Core.Providers.Interfaces;
using CurrencyExchange.Core.Services.Interfaces;

namespace CurrencyExchange.Core.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly IExchangeRateProvider _exchangeRateProvider;

    public ExchangeRateService(IExchangeRateProvider exchangeRateProvider)
    {
        _exchangeRateProvider = exchangeRateProvider;
    }

    public async Task<ExchangeRateDto> GetExchangeRates(string baseCurrency, string[] targetCurrencies)
    {
        var rates = await _exchangeRateProvider.GetExchangeRates(baseCurrency, targetCurrencies);

        var result = new ExchangeRateDto
        {
            LastUpdate = rates.Date,
            Base = rates.Base,
            Rates = rates.Rates
        };

        return result;
    }

    public async Task<ICollection<CurrencyDto>> GetCurrencies()
    {
        var currencies = await _exchangeRateProvider.GetCurrencies();

        var result = currencies.Select(c => new CurrencyDto
            {
                Code = c.Code,
                Name = c.Name,
                DecimalDigits = c.DecimalDigits,
                Symbol = c.Symbol
            })
            .ToList();

        return result;
    }
}