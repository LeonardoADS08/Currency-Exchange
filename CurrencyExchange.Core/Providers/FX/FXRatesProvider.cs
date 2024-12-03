using System.Net.Http.Json;
using CurrencyExchange.Core.Providers.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyExchange.Core.Providers.FX;

// https://fxratesapi.com/
internal class FXRatesProvider : IExchangeRateProvider
{
    // TODO: make this configurable
    private const string ApiUrl = "https://api.fxratesapi.com";
    private const string ProviderName = "FXRatesAPI";

    private readonly HttpClient _httpClient;
    private readonly ILogger<FXRatesProvider> _logger;

    public FXRatesProvider(HttpClient httpClient, ILogger<FXRatesProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Models.ExchangeRate> GetExchangeRates(string baseCurrency, string[] targetCurrencies)
    {
        var targetCurrenciesString = string.Join(",", targetCurrencies);
        var requestUri = $"{ApiUrl}/latest?base={baseCurrency}&symbols={targetCurrenciesString}";

        var response = await _httpClient.GetFromJsonAsync<ExchangeRate>(requestUri);
        if (response == null)
        {
            _logger.LogError("Failed to retrieve exchange rates: {BaseCurrency}/{TargetCurrencies}", baseCurrency, targetCurrenciesString);
            throw new Exception("Failed to retrieve exchange rates.");
        }

        var result = new Models.ExchangeRate
        {
            Provider = ProviderName,
            Date = response.Date,
            Base = response.Base,
            Rates = response.Rates
        };

        return result;
    }

    public async Task<ICollection<Models.Currency>> GetCurrencies()
    {
        var requestUri = $"{ApiUrl}/currencies";
        var response = await _httpClient.GetFromJsonAsync<Dictionary<string, Currency>>(requestUri);
        if (response == null)
        {
            _logger.LogError("Failed to retrieve currencies.");
            throw new Exception("Failed to retrieve currencies.");
        }

        var result = response.Values
            .Select(currency => new Models.Currency
            {
                Code = currency.Code,
                Name = currency.Name,
                DecimalDigits = currency.DecimalDigits,
                Symbol = currency.Symbol
            })
            .ToList();

        return result;
    }
}
