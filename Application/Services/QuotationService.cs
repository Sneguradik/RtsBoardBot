using System.Globalization;
using Domain.Interfaces;
using Domain.Interfaces.RtsBoard;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class QuotationService(IInstrumentRepo instrumentRepo, IMarketDataService marketDataService, IMarketLevelRepo marketLevelRepo, ILogger<QuotationService> logger) :  IQuotationService
{
    public async Task InitAsync(CancellationToken token = default)
    {
        foreach (var instrument in instrumentRepo.GetInstruments()) await marketDataService.Subscribe(instrument);
        logger.LogInformation("QuotationService Initialized");
    }

    public async Task RunAsync(CancellationToken token = default)
    {
        await foreach (var level in marketDataService.GetMarketLevels().WithCancellation(token))
        {
            string ticker;
            if (level.Bid?.Instrument.Ticker != null) ticker = level.Bid.Instrument.Ticker;
            else if (level.Offer?.Instrument.Ticker != null) ticker = level.Offer.Instrument.Ticker;
            else continue;
            //Console.WriteLine($"{level.Bid is null} | {level.Offer is null} | {ticker}");
            marketLevelRepo.Enqueue(ticker,level);
        }
    }
}