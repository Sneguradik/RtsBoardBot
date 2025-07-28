
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Worker;

public class MarketDataWorker(IServiceProvider serviceProvider, IInstrumentRepo instrumentRepo, IOptions<List<string>> tickers , ILogger<MarketDataWorker> logger) : BackgroundService
{
    private readonly IServiceScope _scope = serviceProvider.CreateScope();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var quotationService = scope.ServiceProvider.GetRequiredService<IQuotationService>();
                var marketDataService = scope.ServiceProvider.GetRequiredService<IMarketDataService>();
                var instruments = await marketDataService.GetAllInstruments(stoppingToken);
            
                logger.LogInformation($"Instruments loaded: {instruments.Count()}");

                instrumentRepo.AddInstrument(instruments.Where(x => tickers.Value.Contains(x.Ticker)));
                logger.LogInformation($"Instruments saved. Currently: {instrumentRepo.GetInstruments().Count()}");

                await quotationService.InitAsync(stoppingToken);
                await quotationService.RunAsync(stoppingToken); 
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Worker stopping gracefully.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in MarketDataWorker. Retrying in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _scope.Dispose();
    }
}