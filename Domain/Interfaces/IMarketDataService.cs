using Domain.Entities.Core;

namespace Domain.Interfaces;

public interface IMarketDataService
{
    Task<IEnumerable<Instrument>> GetAllInstruments(CancellationToken token = default);
    Task<Instrument?> GetInstrumentByIsin(string isin, CancellationToken token = default);
    Task Subscribe(Instrument instrument); 
    IAsyncEnumerable<BestLevels> GetMarketLevels();
}