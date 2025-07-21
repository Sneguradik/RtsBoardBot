using Domain.Entities.RTSBoard;

namespace Domain.Interfaces.RtsBoard;

public interface IRtsBoardInstrumentService
{
    void AddInstrument(RtsBoardInstrument instrument);
    RtsBoardInstrument? GetInstrumentByIsin(string isin);
    RtsBoardInstrument? GetInstrumentByTicker(string ticker);
    IEnumerable<RtsBoardInstrument> GetInstruments();
}