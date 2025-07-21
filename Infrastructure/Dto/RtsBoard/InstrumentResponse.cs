using Domain.Entities.RTSBoard;

namespace Infrastructure.Dto.RtsBoard;

public class InstrumentResponse :  BaseResponse
{
    public List<RtsBoardInstrument> Instruments { get; set; } = new();
}