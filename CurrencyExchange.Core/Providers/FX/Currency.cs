using System.Text.Json.Serialization;

namespace CurrencyExchange.Core.Providers.FX;

internal record Currency
{
    public string Code { get; set; }
    public string Name { get; set; }

    [JsonPropertyName("decimal_digits")]
    public int DecimalDigits { get; set; }

    [JsonPropertyName("name_plural")]
    public string NamePlural { get; set; }

    public int Rounding { get; set; }
    public string Symbol { get; set; }
    public string SymbolNative { get; set; }
}