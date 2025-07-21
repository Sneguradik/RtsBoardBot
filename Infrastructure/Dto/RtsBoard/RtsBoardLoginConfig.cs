namespace Infrastructure.Dto.RtsBoard;

public class RtsBoardLoginConfig
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TokenLifetimeInMinutes { get; set; }
}