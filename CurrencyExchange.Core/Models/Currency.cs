namespace CurrencyExchange.Core.Models;

public record Currency
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int DecimalDigits { get; set; }
    public string Symbol { get; set; } = null!;
}