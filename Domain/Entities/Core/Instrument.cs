namespace Domain.Entities.Core;

public class Instrument
{
    public string InstrumentUid { get; set; } = string.Empty;
    public string Isin { get; set; } = string.Empty;
    public string Ticker { get; set; } = string.Empty;
}