using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using Domain.Interfaces.RtsBoard;
using Infrastructure.Dto.RtsBoard;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class RtsBoardAuthService(HttpClient client, IOptions<RtsBoardLoginConfig> conf, ILogger<RtsBoardAuthService> logger): IRtsBoardAuthService
{
    private DateTime? _lastTimeUpdated = null;
    private string? _refreshToken = null;
    private string? _accessToken = null;
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private async Task<string> AuthAsync<T>(T payload, Uri uri, CancellationToken cancellationToken = default)
    {
        var msg = new HttpRequestMessage()
        {
            RequestUri = uri,
            Method = HttpMethod.Post,
            Content = JsonContent.Create(payload)
        };
        var response = await client.SendAsync(msg, cancellationToken);
        logger.LogInformation("RtsBoardAuthService: Sent {StatusCode}.", response.StatusCode);
        if (!response.IsSuccessStatusCode)throw new Exception(response.ReasonPhrase);
        var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var content = JsonSerializer.Deserialize<OAuthTokenResponse>(stringContent, _jsonSerializerOptions);
        if (content is null) throw new SerializationException("Could not deserialize response");
        _lastTimeUpdated = DateTime.UtcNow;
        _refreshToken = content.RefreshToken;
        return content.AccessToken;
    }
    
    public async Task<string> GetAuthToken(CancellationToken token = default)
    {
        logger.LogInformation("Login requested.");
        _accessToken =  await Login(conf.Value.Username, conf.Value.Password, token);
        return _accessToken;
        if (_accessToken is null || _refreshToken is null)
        {
            logger.LogInformation("Login requested.");
            _accessToken =  await Login(conf.Value.Username, conf.Value.Password, token);
            return _accessToken;
        }

        if (!(DateTime.UtcNow - _lastTimeUpdated > TimeSpan.FromMinutes(conf.Value.TokenLifetimeInMinutes)))
            return _accessToken;
        logger.LogInformation("Refresh token requested.");
        _accessToken = await RefreshToken(_refreshToken, token);
        return _accessToken;
    }

    private async Task<string> Login(string username, string password, CancellationToken token = default) => 
        await AuthAsync(new RtsBoardLoginDto{Username = username, Password = password, Signature = ""}, 
            new Uri("/api/token/get", UriKind.Relative), token);
    

    private async Task<string> RefreshToken(string refreshToken, CancellationToken token = default) =>
        await AuthAsync(new RtsBoardRefreshDto(refreshToken), new Uri("/api/token/refresh", UriKind.Relative), token);
}