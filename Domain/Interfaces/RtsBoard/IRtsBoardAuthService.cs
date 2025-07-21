namespace Domain.Interfaces.RtsBoard;

public interface IRtsBoardAuthService
{
    Task<string> GetAuthToken(CancellationToken token = default);
}