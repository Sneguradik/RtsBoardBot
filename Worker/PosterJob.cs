using Domain.Entities.Core;
using Domain.Entities.RTSBoard;
using Domain.Errors;
using Domain.Interfaces;
using Domain.Interfaces.RtsBoard;
using Quartz;

namespace Worker;

public class PosterJob(IRtsBoardOpenDealsRepo boardOpenDealsRepo, IRtsBoardTradingService tradingService, IMarketLevelRepo marketLevelRepo, IRtsBoardInstrumentService rtsBoardInstrument, ILogger<PosterJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        
        foreach (var ticker in marketLevelRepo.GetTickers())
        {
            try
            {
                await ProcessTickerAsync(ticker, context.CancellationToken);
            }
            catch (WrongRevisionException e)
            {
                var doc = boardOpenDealsRepo.GetOpenDeal(e.DealId);
                if (doc is null) continue;
                doc.Rev = e.Revision;
                boardOpenDealsRepo.UpdateOpenDeal(doc);
                logger.LogCritical("Revision exception");
                await ProcessTickerAsync(ticker, context.CancellationToken);
            }
            catch (UnauthorizedAccessException e)
            {
                logger.LogCritical("Unauthorized access exception");
                await ProcessTickerAsync(ticker, context.CancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
            
        }
    
        
    }

    private async Task ProcessTickerAsync(string ticker, CancellationToken ct = default)
    {
        var bestLevels = marketLevelRepo.GetBestLevels(ticker);
        if (bestLevels is null) return;
        var deals = boardOpenDealsRepo.GetOpenDealsByTicker(ticker);
        var buyDeal = deals
            .FirstOrDefault(x => x.Direction.Equals(nameof(LevelDirection.Buy), 
                StringComparison.CurrentCultureIgnoreCase));
        var sellDeal= deals
            .FirstOrDefault(x => x.Direction.Equals(nameof(LevelDirection.Sell), 
                StringComparison.CurrentCultureIgnoreCase));
        RtsBoardInstrument?  instrument;
        if (bestLevels.Bid?.Instrument.Ticker != null) instrument = rtsBoardInstrument.GetInstrumentByTicker(bestLevels.Bid.Instrument.Ticker);
        else if (bestLevels.Offer?.Instrument.Ticker != null) instrument = rtsBoardInstrument.GetInstrumentByTicker(bestLevels.Offer.Instrument.Ticker);
        else return;
        
        if(instrument is null) return;

        if (bestLevels.Bid is not null) await ProcessMarketLevel(bestLevels.Bid, buyDeal, instrument, ct);
        if (bestLevels.Offer is not null) await ProcessMarketLevel(bestLevels.Offer, sellDeal, instrument, ct);
    }

    private async Task ProcessMarketLevel(MarketLevel marketLevel, RtsBoardQuote? quote, RtsBoardInstrument instrument, CancellationToken ct)
    {
        logger.LogInformation($"Started processing market level for {marketLevel.Instrument.Ticker} / {marketLevel.Instrument.Isin}, {marketLevel.Direction.ToString()} price - {marketLevel.Price} quantity - {marketLevel.Volume} quote - {quote?.Id}");
        RtsBoardQuote? tempQuote;
        if (quote is null) tempQuote = await tradingService.CreateQuote(marketLevel, ct);
        else
        {
            tempQuote = await tradingService.CancelQuote(quote.Id, quote.Rev,ct);
            tempQuote.Price = marketLevel.Price;
            tempQuote.Quantity = marketLevel.Volume;
            tempQuote.SettlementDate = DateTime.UtcNow+TimeSpan.FromMinutes(5);
            tempQuote.DeliveryDate = DateTime.UtcNow+TimeSpan.FromMinutes(5);
            tempQuote.GenerateContent(instrument);
            tempQuote = await tradingService.UpdateQuote(tempQuote, ct);
        }

        if (tempQuote is not null)
        {
            var oldQ = tempQuote;
            tempQuote = await tradingService.SendQuote(tempQuote.Id, tempQuote.Rev, ct);
            if (tempQuote.Rev<=oldQ.Rev) tempQuote.Rev = oldQ.Rev+1;
            if (quote is null) boardOpenDealsRepo.AddOpenDeal(tempQuote);
            else boardOpenDealsRepo.UpdateOpenDeal(tempQuote);
            logger.LogInformation($"Successfully processed {tempQuote?.Id}");
        }
        else logger.LogError($"Failed to process market level for {marketLevel.Instrument.Ticker} / {marketLevel.Instrument.Isin}, {marketLevel.Direction.ToString()}");
        
    }

}