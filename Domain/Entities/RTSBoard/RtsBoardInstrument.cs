namespace Domain.Entities.RTSBoard;

public class RtsBoardInstrument
{
    public string Id { get; set; } =  string.Empty;
    public string Isin { get; set; } = string.Empty;
    public ulong? IssuingVolume { get; set; }
    public LocalizedText Name { get; set; } = new();
    public string Currency { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string XmlElementName { get; set; } = string.Empty;
    public bool AllowOrderBook { get; set; }
    public bool IsActive { get; set; }
    public string ProductTypeId { get; set; } = string.Empty;
    public bool EnableMarketData { get; set; }
    public DateTime? DateAdding { get; set; }
}