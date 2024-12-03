using CurrencyExchange.Core.Models;
using CurrencyExchange.Core.Models.Response;
using CurrencyExchange.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyExchangeController : ControllerBase
{
    private readonly IExchangeRateService _exchangeService;

    public CurrencyExchangeController(IExchangeRateService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpGet("rates")]
    public async Task<ActionResult<ExchangeRateDto>> GetExchangeRates([FromQuery] string baseCurrency, [FromQuery] string[] targetCurrencies)
    {
        var rates = await _exchangeService.GetExchangeRates(baseCurrency, targetCurrencies);
        return Ok(rates);
    }

    [HttpGet("currencies")]
    public async Task<ActionResult<ICollection<Currency>>> GetCurrencies()
    {
        var currencies = await _exchangeService.GetCurrencies();
        return Ok(currencies);
    }
}