using Domain.Entities.RTSBoard;
using Domain.Interfaces.RtsBoard;

namespace Infrastructure.Services;

public class RtsBoardInstrumentService : IRtsBoardInstrumentService
{
    private readonly List<RtsBoardInstrument> _instruments = new ();
    public void AddInstrument(RtsBoardInstrument instrument) => _instruments.Add(instrument);

    public RtsBoardInstrument? GetInstrumentByIsin(string isin) => _instruments
        .FirstOrDefault(i => i.Isin == isin);
    public RtsBoardInstrument? GetInstrumentByTicker(string ticker) => _instruments
        .FirstOrDefault(i=>i.Id == ticker);

    public IEnumerable<RtsBoardInstrument> GetInstruments() => _instruments;
}