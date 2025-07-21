using System.Collections.Concurrent;
using Domain.Entities.Core;
using Domain.Interfaces;

namespace Application.Services;

public class MarketLevelRepo : IMarketLevelRepo, IDisposable
{
    private readonly BlockingCollection<(string Ticker, BestLevels Levels)> _queue = new(2048);
    private readonly ConcurrentDictionary<string, BestLevels> _levels = new();
    private Task? _worker;

    public Task InitAsync(CancellationToken token)
    {
        _worker = Task.Run(() => ProcessQueueAsync(token), token);
        return Task.CompletedTask;
    }

    public void Enqueue(string ticker, BestLevels levels)
    {
        if (!string.IsNullOrWhiteSpace(ticker))
            _queue.TryAdd((ticker, levels));
    }

    public BestLevels? GetBestLevels(string ticker) =>
        _levels.GetValueOrDefault(ticker);

    public IEnumerable<string> GetTickers() => _levels.Keys;

    private async Task ProcessQueueAsync(CancellationToken token)
    {
        try
        {
            foreach (var (ticker, levels) in _queue.GetConsumingEnumerable(token))
            {
                _levels.AddOrUpdate(ticker, levels, (_, _) => levels);
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
    }

    public void Dispose()
    {
        _queue.CompleteAdding();
        _worker?.Wait();
    }
}