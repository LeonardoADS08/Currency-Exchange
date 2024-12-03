namespace CurrencyExchange.Core.Providers.FX;

internal record ExchangeRate
{
    public bool Success { get; init; }
    public string Terms { get; init; } = string.Empty;
    public string Privacy { get; init; } = string.Empty;
    public long Timestamp { get; init; }
    public DateTime Date { get; init; }
    public string Base { get; init; } = string.Empty;
    public Dictionary<string, decimal> Rates { get; init; } = new();
}