namespace CurrencyExchange.Core.Models.Response;

public record RateInfoDto
{
    public string Code { get; set; }
    public decimal Value { get; set; }
    public decimal? ChangePercentage { get; set; }
}