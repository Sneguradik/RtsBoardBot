namespace Domain.Interfaces.RtsBoard;

public interface IRtsBoardOpenDealsRepo
{
    void AddOpenDeal(RtsBoardQuote openDeal);
    IEnumerable<RtsBoardQuote> GetOpenDealsByTicker(string ticker);
    IEnumerable<RtsBoardQuote> GetOpenDeals();
    RtsBoardQuote? GetOpenDeal(string id);
    void UpdateOpenDeal(RtsBoardQuote openDeal);
    void DeleteOpenDeal(string id);
}