using System.Collections.Concurrent;
using Domain.Interfaces.RtsBoard;

namespace Application.Services;

public class RtsBoardOpenDealsRepo : IRtsBoardOpenDealsRepo
{
    private readonly ConcurrentDictionary<string, RtsBoardQuote> _deals = new();

    public void AddOpenDeal(RtsBoardQuote openDeal)
    {
        if (!string.IsNullOrWhiteSpace(openDeal.Id)) _deals[openDeal.Id] = openDeal;
    }

    public IEnumerable<RtsBoardQuote> GetOpenDealsByTicker(string ticker) => _deals.Values
        .Where(deal => deal.Instrument.Equals(ticker, StringComparison.OrdinalIgnoreCase));
    

    public IEnumerable<RtsBoardQuote> GetOpenDeals() => _deals.Values;

    public RtsBoardQuote? GetOpenDeal(string id) => _deals.GetValueOrDefault(id);

    public void UpdateOpenDeal(RtsBoardQuote openDeal)
    {
        if (!string.IsNullOrWhiteSpace(openDeal.Id)) _deals[openDeal.Id] = openDeal;
    }

    public void DeleteOpenDeal(string id) => _deals.TryRemove(id, out _);
}