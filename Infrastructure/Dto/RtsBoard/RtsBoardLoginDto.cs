using System.Text.Json.Serialization;

namespace Infrastructure.Dto.RtsBoard;

public class RtsBoardLoginDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = "password";
    public string Username { get; set; } =  string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}