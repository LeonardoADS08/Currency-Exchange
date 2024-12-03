using CurrencyExchange.Core.Models;
using CurrencyExchange.Core.Providers.Interfaces;
using CurrencyExchange.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyExchange.Tests;

public class ExchangeRateServiceTests
{
    [Theory]
    [InlineData("USD", new[] { "EUR", "GBP" })]
    [InlineData("EUR", new[] { "USD", "JPY" })]
    [InlineData("GBP", new[] { "USD", "EUR" })]
    public async Task GetExchangeRatesAsync_WithValidData_ReturnsExchangeRate(string baseCurrency, string[] targetCurrencies)
    {
        // Arrange
        var exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
        var exchangeRate = new ExchangeRate
        {
            Base = baseCurrency,
            Date = DateTime.UtcNow,
            Rates = targetCurrencies.ToDictionary(currency => currency, currency => 1.0m)
        };

        exchangeRateProviderMock
            .Setup(provider => provider.GetExchangeRates(baseCurrency, targetCurrencies))
            .ReturnsAsync(exchangeRate);

        var exchangeRateService = new ExchangeRateService(exchangeRateProviderMock.Object);

        // Act
        var result = await exchangeRateService.GetExchangeRates(baseCurrency, targetCurrencies);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(baseCurrency, result.Base);
        Assert.Equal(targetCurrencies.Length, result.Rates.Count);
    }

    [Fact]
    public async Task GetCurrenciesAsync_WithValidData_ReturnsCurrencies()
    {
        // Arrange
        var exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
        var currencies = new List<Currency>
        {
            new Currency { Code = "USD", Name = "United States Dollar", DecimalDigits = 2, Symbol = "$" },
            new Currency { Code = "EUR", Name = "Euro", DecimalDigits = 2, Symbol = "€" },
            new Currency { Code = "GBP", Name = "British Pound", DecimalDigits = 2, Symbol = "£" }
        };

        exchangeRateProviderMock
            .Setup(provider => provider.GetCurrencies())
            .ReturnsAsync(currencies);

        var exchangeRateService = new ExchangeRateService(exchangeRateProviderMock.Object);

        // Act
        var result = await exchangeRateService.GetCurrencies();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(currencies.Count, result.Count);
    }
}
