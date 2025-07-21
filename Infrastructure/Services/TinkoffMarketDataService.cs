using System.Globalization;
using System.Text.Json;
using Domain.Entities.Core;
using Domain.Interfaces;
using Grpc.Core;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Instrument = Domain.Entities.Core.Instrument;

namespace Infrastructure.Services;

public class TinkoffMarketDataService(InvestApiClient client, IInstrumentRepo instrumentRepo) : IMarketDataService
{
    private readonly InstrumentsService.InstrumentsServiceClient _instrumentsServiceClient = client.Instruments;
    private readonly AsyncDuplexStreamingCall<MarketDataRequest,MarketDataResponse> _marketDataStream = client.
        MarketDataStream.MarketDataStream();

    private static Instrument ConvertShareToInstrument(Share share) => new()
    {
        Isin = share.Isin,
        Ticker = share.Ticker,
        InstrumentUid = share.Uid,
    };

    private static double PriceConverter(Quotation quotation) =>
        Convert.ToDouble($"{quotation.Units}.{quotation.Nano}", CultureInfo.InvariantCulture);

    private static MarketLevel ConvertOrdersToMarketLevel(Order order, Instrument instrument, LevelDirection direction) => new ()
    {
        Direction = direction,
        Instrument = instrument,
        Price = PriceConverter(order.Price),
        Volume = order.Quantity
    };

    public async Task<IEnumerable<Instrument>> GetAllInstruments(CancellationToken token = default)
    {
        var instruments = (await _instrumentsServiceClient.SharesAsync(token)).Instruments;
        return instruments.Select(ConvertShareToInstrument);
    }
        
  
    public async Task<Instrument?> GetInstrumentByIsin(string isin,CancellationToken token = default)
    {
        var share = await _instrumentsServiceClient.ShareByAsync(new InstrumentRequest(){IdType = InstrumentIdType.Uid, Id = isin});
        return share is null ? null : ConvertShareToInstrument(share.Instrument);
    }
    public async Task Subscribe(Instrument instrument)
    {
        await _marketDataStream.RequestStream.WriteAsync(new MarketDataRequest()
        {
            SubscribeOrderBookRequest = new SubscribeOrderBookRequest()
            {
                Instruments = { new OrderBookInstrument()
                {
                    
                    InstrumentId = instrument.InstrumentUid,
                    Depth = 1,
                    OrderBookType = OrderBookType.All
                } },
                SubscriptionAction = SubscriptionAction.Subscribe
            }
        });
    }

    public async IAsyncEnumerable<BestLevels> GetMarketLevels()
    {
        await foreach (var orderBook in _marketDataStream.ResponseStream.ReadAllAsync())
        {
            if (orderBook.Orderbook is null) continue;
            var instrument = instrumentRepo.GetInstrumentByUid(orderBook.Orderbook.InstrumentUid);
    
            var bestBid = orderBook.Orderbook.Bids.FirstOrDefault();
            var bestAsk = orderBook.Orderbook.Asks.FirstOrDefault();
            if (instrument is null) continue;
            
            yield return new BestLevels(bestBid is not null? 
                ConvertOrdersToMarketLevel(bestBid, instrument, LevelDirection.Buy):null, 
                bestAsk is not null? 
                    ConvertOrdersToMarketLevel(bestAsk, instrument, LevelDirection.Sell):null);

        }
    }
}