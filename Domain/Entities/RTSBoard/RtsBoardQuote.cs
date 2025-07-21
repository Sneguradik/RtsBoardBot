using System.Globalization;
using System.Text.Json.Serialization;
using Domain.Entities.Core;
using Domain.Entities.RTSBoard;

public class Error
{
    public string? Id { get; set; }
    public LocalizedText Message { get; set; } = new ();
}


public class RtsBoardQuote
{
    public string Instrument { get; set; } = string.Empty;
    public Trader Trader { get; set; } = new ();
    public string Party { get; set; } = string.Empty;
    public List<string>? CounterpartyIds { get; set; } = [];
    public bool HasCounterparties { get; set; }
    public bool IsTargeted { get; set; }
    public bool IsDynamic { get; set; }
    public object QuoteRequestId { get; set; }
    public object FrontTradeId { get; set; }
    public string Comment { get; set; }
    public string ClientCode { get; set; }
    public double Price { get; set; }
    public double? StandardPrice { get; set; }
    public double Quantity { get; set; }
    public string Direction { get; set; } = string.Empty;
    public bool IsIndicative { get; set; }
    public string PriceCurrency { get; set; } = string.Empty;
    public string NominalCurrency { get; set; } = string.Empty;
    public bool IsPartialExecution { get; set; }
    public bool IsInformationQuote { get; set; }
    public bool IsAnonymousQuote { get; set; }
    public bool ShowIfTheBest { get; set; }
    public string TimeToLive { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public string DeliveryMethod { get; set; } = string.Empty;
    public DateTime SettlementDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string SettlementCurrency { get; set; } = string.Empty;
    public double ExchangeRate { get; set; }
    public string SettlementPlace { get; set; } = string.Empty;
    public object Details { get; set; }
    public string Id { get; set; } = string.Empty;
    public int Rev { get; set; }
    public object Marks { get; set; }
    public QuoteState State { get; set; } = new ();
    public bool? IsValid { get; set; }
    public LocalizedText Description { get; set; }  = new ();
    public string? Content { get; set; }
    public string SchemaVersion { get; set; } = string.Empty;
    public string? RowVersion { get; set; }
    public List<string>? Actions { get; set; }
    [JsonPropertyName("$dirty")]
    public bool? Dirty { get; set; }
    
    public void GenerateContent(RtsBoardInstrument boardInstrument)
    {
        Content =
            $"<rtsotc:equityTransaction id=\"EquityTransaction1\" xmlns=\"http://www.fpml.org/FpML-5/confirmation\" xmlns:rtsotc=\"http://www.fpml.ru/otc-system\" xmlns:rtsrep=\"http://www.fpml.ru/repository\" xmlns:fpmlext=\"http://www.fpml.org/FpML-5/ext\" xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"><productType>SecurityTransaction</productType><productId>{boardInstrument.Id}</productId><buyerPartyReference href=\"{(Direction == nameof(LevelDirection.Buy).ToUpper() ? "quote-owner" : "counterparty")}\"/><sellerPartyReference href=\"{(Direction == nameof(LevelDirection.Buy).ToUpper() ? "counterparty" : "quote-owner")}\"/><rtsotc:issuingVolumes>{boardInstrument.IssuingVolume}</rtsotc:issuingVolumes><rtsotc:numberOfUnits>{Quantity.ToString(CultureInfo.InvariantCulture)}</rtsotc:numberOfUnits><rtsotc:unitPrice>{Price.ToString(CultureInfo.InvariantCulture)}</rtsotc:unitPrice><rtsotc:priceCurrency>{boardInstrument.Currency}</rtsotc:priceCurrency><rtsotc:equity id=\"{boardInstrument.Id}\"><instrumentId instrumentIdScheme=\"http://www.fpml.ru/coding-scheme/instrument-id#code\">{boardInstrument.Id}</instrumentId><instrumentId instrumentIdScheme=\"http://www.fpml.ru/coding-scheme/instrument-id#regnum\">1-02-12500-A</instrumentId><instrumentId instrumentIdScheme=\"http://www.fpml.ru/coding-scheme/instrument-id#isin\">{boardInstrument.Isin}</instrumentId><description>{boardInstrument.Name.Ru}</description><currency id=\"Currency1\">{boardInstrument.Currency}</currency></rtsotc:equity><rtsotc:unitNotional>1.00000</rtsotc:unitNotional><rtsotc:deliveryMethod>DeliveryVersusPayment</rtsotc:deliveryMethod><rtsotc:settlementDate>{DateTime.UtcNow:yyyy-MM-dd}</rtsotc:settlementDate><rtsotc:deliveryDate>{DateTime.UtcNow:yyyy-MM-dd}</rtsotc:deliveryDate><rtsotc:settlementCurrency>{boardInstrument.Currency}</rtsotc:settlementCurrency></rtsotc:equityTransaction>";
    }
    
}





