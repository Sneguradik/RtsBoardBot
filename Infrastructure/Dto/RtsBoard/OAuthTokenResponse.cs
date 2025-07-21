namespace Infrastructure.Dto.RtsBoard;

public class OAuthTokenResponse : BaseResponse
{
    public string Error { get; set; } =  string.Empty;
    public string AccessToken { get; set; } =  string.Empty;
    public string RefreshToken { get; set; } =  string.Empty;   
    public string TokenType { get; set; } =  string.Empty;
}