namespace CurrencyExchange.Core.Models.Response;

public record ExchangeRateDto
{
    public DateTime LastUpdate { get; set; }
    public string Base { get; set; } = string.Empty;
    public IDictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
}