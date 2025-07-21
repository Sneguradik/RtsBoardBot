namespace Domain.Entities.RTSBoard;

public class QuoteState
{
    public string Id { get; set; } = string.Empty;
    public string LockState { get; set; } = string.Empty;
    public List<string> Actions { get; set; }  = [];
    public Error Error { get; set; } = new ();
    public DateTime ChangeDate { get; set; }
    public int? ChangedBy { get; set; }
}