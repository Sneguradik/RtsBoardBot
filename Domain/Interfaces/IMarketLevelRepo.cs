using Domain.Entities.Core;

namespace Domain.Interfaces;

public interface IMarketLevelRepo
{
    Task InitAsync(CancellationToken token);
    void Enqueue(string ticker, BestLevels levels);
    BestLevels? GetBestLevels(string ticker);
    IEnumerable<string> GetTickers();
}

public record BestLevels(MarketLevel? Bid, MarketLevel? Offer);