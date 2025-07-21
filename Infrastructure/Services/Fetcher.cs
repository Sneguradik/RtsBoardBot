using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Domain.Errors;
using Domain.Interfaces.RtsBoard;
using Infrastructure.Dto.RtsBoard;

namespace Infrastructure.Services;

public class Fetcher(HttpClient httpClient, IRtsBoardAuthService  authService)
{
    protected virtual JsonSerializerOptions JsonSerializerOptions { get; } = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    protected async Task<TRes> SendAsync<TReq, TRes>(TReq payload, Uri uri, CancellationToken cancellationToken = default)
    {
        var reqMsg = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = JsonContent.Create(payload, new MediaTypeHeaderValue("application/json"),  JsonSerializerOptions)
        };

        reqMsg.Headers.TryAddWithoutValidation("Authorization",
            $"Bearer {await authService.GetAuthToken(cancellationToken)}");
        
        var res = await httpClient.SendAsync(reqMsg, cancellationToken);

        if (!res.IsSuccessStatusCode)
        {
            if (res.StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedAccessException();
            else throw new Exception(await res.Content.ReadAsStringAsync(cancellationToken));
        }
        
        var respStream = await res.Content.ReadAsStreamAsync(cancellationToken);
        
        await CheckForErrors(respStream,  cancellationToken);
        
        var content =  await JsonSerializer
            .DeserializeAsync<TRes>(respStream, JsonSerializerOptions, cancellationToken);
        
        if(content is null) throw new SerializationException("Content is null. Error in serialization.");
        
        return content;
    }

    private async Task CheckForErrors( Stream content, CancellationToken token = default)
    {
        content.Seek(0, SeekOrigin.Begin);
        
        var success = await JsonSerializer
            .DeserializeAsync<BaseResponse>(content, JsonSerializerOptions, token);
        
        if (success is null) throw new SerializationException("Content is null. Error in serialization.");

        if (!success.Success)
        {
            using var sr = new StreamReader(content);
            if (success.Error is null) throw new Exception(await sr.ReadToEndAsync(token));

            if (success.Error == "E_OUTDATED_REVISION" && success.Message is not null)
            {
                var input = success.Message.En;

                var match = Regex.Match(input, @"QUOTE:([A-Z0-9\-]+).*?Actual revision is #(\d+)");
                if (match.Success)
                {
                    string documentId = match.Groups[1].Value;
                    var revision = int.Parse(match.Groups[2].Value);
                    throw new WrongRevisionException() { Revision = revision, DealId = documentId };
                }
            }
            
            throw new Exception(await sr.ReadToEndAsync(token));

        }
        
        content.Seek(0, SeekOrigin.Begin);
    }
}