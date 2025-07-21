
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Worker;

public class MarketDataWorker(IServiceProvider serviceProvider, IInstrumentRepo instrumentRepo, IOptions<List<string>> tickers , ILogger<MarketDataWorker> logger) : BackgroundService
{
    private readonly IServiceScope _scope = serviceProvider.CreateScope();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var quotationService = _scope.ServiceProvider.GetRequiredService<IQuotationService>();
        var marketDataService = _scope.ServiceProvider.GetRequiredService<IMarketDataService>();
        
        var instruments = await marketDataService.GetAllInstruments(stoppingToken);
        
        logger.LogInformation($"Instruments loaded : {instruments.Count()}");
        
        instrumentRepo.AddInstrument(instruments.Where(x=>tickers.Value.Contains(x.Ticker)));
        
        logger.LogInformation($"Instruments saved. Currently : {instrumentRepo.GetInstruments().Count()}");

        await quotationService.InitAsync(stoppingToken);
        await quotationService.RunAsync(stoppingToken);
    }

    public override void Dispose()
    {
        base.Dispose();
        _scope.Dispose();
    }
}