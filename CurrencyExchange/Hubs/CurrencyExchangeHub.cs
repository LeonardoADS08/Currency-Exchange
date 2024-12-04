using CurrencyExchange.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace CurrencyExchange.API.Hubs;

public class CurrencyExchangeHub : Hub
{
    public async Task SendExchangeRateUpdate(ExchangeRate update)
    {
        await Clients.All.SendAsync("ReceiveExchangeRateUpdate", update);
    }
}