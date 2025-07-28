using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using Domain.Entities.Core;
using Domain.Entities.RTSBoard;
using Domain.Interfaces.RtsBoard;
using Infrastructure.Dto.RtsBoard;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class RtsBoardTradingService(HttpClient httpClient, IRtsBoardAuthService authService, IRtsBoardInstrumentService instrumentService, 
    IRtsBoardOpenDealsRepo openDealsRepo, ILogger<RtsBoardTradingService> logger) : Fetcher(httpClient, authService), IRtsBoardTradingService
{
   
    
    
    public async Task<RtsBoardQuote?> CreateQuote(MarketLevel level, CancellationToken token = default)
    {
        var boardInstrument = instrumentService.GetInstrumentByTicker(level.Instrument.Ticker);
        if (boardInstrument?.IssuingVolume is null) return null;
        
        var quote = new RtsBoardQuote()
        {
            Instrument = boardInstrument.Id,
            Party = "Tech1",
            Trader = new Trader() { Id = 1 },
            Rev = 1,
            State = new()
            {
                LockState = "RELEASED",
                Actions = ["create"],
                ChangeDate = DateTime.UtcNow,
                ChangedBy = null
            },
            CounterpartyIds = null,
            Price = level.Price,
            PriceCurrency = "RUB",
            Quantity = level.Volume,
            Direction = level.Direction.ToString().ToUpper(),
            ClientCode = "проверка123",
            Comment = $"Quote for {boardInstrument.Id}",
            SettlementPlace = "SPB",
            TimeToLive = "GTD",
            ExpirationDate = DateTime.UtcNow + TimeSpan.FromDays(1),
            IsPartialExecution = true,
            Actions = ["create"],
            Dirty = true,
        };
        quote.GenerateContent(boardInstrument);

        var resp = await SendAsync<CreateQuoteDto, QuoteResponseDto>(new CreateQuoteDto(){Document = quote}, new Uri("/api/quote/create", UriKind.Relative), token);
        return resp.Document;
    }

    public async Task<RtsBoardQuote> UpdateQuote(RtsBoardQuote quote, CancellationToken token = default) =>
        (await SendAsync<UpdateQuoteDto,QuoteResponseDto>(new UpdateQuoteDto(quote.Id,quote.Rev, quote),new Uri("/api/quote/update", UriKind.Relative),
            token)).Document;

    public async Task<RtsBoardQuote> GetQuote(string id, int rev, CancellationToken token = default)=>
        (await SendAsync<MutateQuoteDto, QuoteResponseDto>(new MutateQuoteDto(id, rev), new Uri("/api/quote/get", UriKind.Relative),
            token)).Document;
    
    public async Task<RtsBoardQuote> SendQuote(string id, int rev, CancellationToken token = default)=>
        (await SendAsync<MutateQuoteDto, QuoteResponseDto>(new MutateQuoteDto(id, rev), new Uri("/api/quote/send", UriKind.Relative),
            token)).Document;

    public async Task<RtsBoardQuote> CancelQuote(string id, int rev, CancellationToken token = default)=>
        (await SendAsync<MutateQuoteDto, QuoteResponseDto>(new MutateQuoteDto(id, rev), new Uri("/api/quote/cancel",  UriKind.Relative),
            token)).Document;

    public async Task<RtsBoardQuote> DeleteQuote(string id, int rev, CancellationToken token = default) =>
        (await SendAsync<MutateQuoteDto, QuoteResponseDto>(new MutateQuoteDto(id, rev), new Uri("/api/quote/delete", UriKind.Relative),
            token)).Document;
}
