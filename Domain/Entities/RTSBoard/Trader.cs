namespace Domain.Entities.RTSBoard;

public class Trader
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string FirstName { get; set; }  = string.Empty;
    public string LastName { get; set; } = string.Empty;
}