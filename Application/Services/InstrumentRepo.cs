using Domain.Entities.Core;
using Domain.Interfaces;

namespace Application.Services;

using System.Collections.Concurrent;

public class InstrumentRepo : IInstrumentRepo
{
    private readonly ConcurrentDictionary<string, Instrument> _isinMap = new();
    private readonly ConcurrentDictionary<string, Instrument> _tickerMap = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Instrument> _uidMap = new();

    public Instrument? GetInstrumentByIsin(string isin)
    {
        _isinMap.TryGetValue(isin, out var instrument);
        return instrument;
    }

    public Instrument? GetInstrumentByTicker(string ticker)
    {
        _tickerMap.TryGetValue(ticker, out var instrument);
        return instrument;
    }

    public Instrument? GetInstrumentByUid(string uid)
    {
        _uidMap.TryGetValue(uid, out var instrument);
        return instrument;
    }

    public IEnumerable<Instrument> GetInstruments() => _isinMap.Values.ToArray();

    public void AddInstrument(Instrument instrument)
    {
        if (!string.IsNullOrWhiteSpace(instrument.Isin))
            _isinMap[instrument.Isin] = instrument;

        if (!string.IsNullOrWhiteSpace(instrument.Ticker))
            _tickerMap[instrument.Ticker] = instrument;

        if (!string.IsNullOrWhiteSpace(instrument.InstrumentUid))
            _uidMap[instrument.InstrumentUid] = instrument;
    }

    public void AddInstrument(IEnumerable<Instrument> instruments)
    {
        foreach (var instrument in instruments)
            AddInstrument(instrument);
    }

    public void DeleteInstrument(Instrument instrument)
    {
        if (!string.IsNullOrWhiteSpace(instrument.Isin))
            _isinMap.TryRemove(instrument.Isin, out _);

        if (!string.IsNullOrWhiteSpace(instrument.Ticker))
            _tickerMap.TryRemove(instrument.Ticker, out _);

        if (!string.IsNullOrWhiteSpace(instrument.InstrumentUid))
            _uidMap.TryRemove(instrument.InstrumentUid, out _);
    }

    public void DeleteInstrument(IEnumerable<Instrument> instruments)
    {
        foreach (var instrument in instruments)
            DeleteInstrument(instrument);
    }
}
