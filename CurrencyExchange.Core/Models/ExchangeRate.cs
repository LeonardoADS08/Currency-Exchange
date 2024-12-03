namespace CurrencyExchange.Core.Models;

public record ExchangeRate
{
    public string Provider { get; init; } = null!;
    public DateTime Date { get; init; }
    public string Base { get; init; } = null!;
    public Dictionary<string, decimal> Rates { get; init; } = new();
}