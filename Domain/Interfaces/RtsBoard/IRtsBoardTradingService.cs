using Domain.Entities.Core;

namespace Domain.Interfaces.RtsBoard;

public interface IRtsBoardTradingService
{
    Task<RtsBoardQuote?> CreateQuote(MarketLevel level, CancellationToken token = default);
    Task<RtsBoardQuote> UpdateQuote(RtsBoardQuote quote, CancellationToken token = default);
    Task<RtsBoardQuote>  GetQuote(string id, int rev, CancellationToken token = default);
    Task<RtsBoardQuote> SendQuote(string id, int rev, CancellationToken token = default);
    Task<RtsBoardQuote> CancelQuote(string id, int rev, CancellationToken token = default);
    Task<RtsBoardQuote> DeleteQuote(string id, int rev, CancellationToken token = default);
}