using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using Domain.Entities.RTSBoard;
using Domain.Interfaces.RtsBoard;
using Infrastructure.Dto.RtsBoard;
using Infrastructure.Services;

namespace Infrastructure.Repos;

public class RtsBoardInstrumentRepo(HttpClient client, IRtsBoardAuthService authService) : Fetcher(client, authService), IRtsBoardInstrumentRepo
{
   
    

    public async Task<IEnumerable<RtsBoardInstrument>> GetAllInstruments(CancellationToken token = default)
    {
        var accessToken = await authService.GetAuthToken(token);
        Console.WriteLine(accessToken);
        var content = await SendAsync< InstrumentRequest,InstrumentResponse>(new InstrumentRequest(){IncludeInactive = false}, new Uri("/api/instrument/list", UriKind.Relative), token);
        return content.Instruments;
    }
}