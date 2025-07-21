using Domain.Entities.RTSBoard;

namespace Domain.Interfaces.RtsBoard;

public interface IRtsBoardInstrumentRepo
{
    Task<IEnumerable<RtsBoardInstrument>> GetAllInstruments(CancellationToken token = default);
}