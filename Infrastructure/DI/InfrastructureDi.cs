using System.Net.Http.Headers;
using Domain.Interfaces;
using Domain.Interfaces.RtsBoard;
using Infrastructure.Repos;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class InfrastructureDi
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string tinkoffToken)
    {
        services.AddInvestApiClient((_, settings) => { settings.AccessToken = tinkoffToken; });
        services.AddHttpClient<IRtsBoardAuthService, RtsBoardAuthService>(ConfigureRtsBoardHttpClient);
        services.AddHttpClient<IRtsBoardInstrumentRepo,RtsBoardInstrumentRepo>(ConfigureRtsBoardHttpClient);
        services.AddHttpClient<IRtsBoardTradingService, RtsBoardTradingService>(ConfigureRtsBoardHttpClient);
        services.AddScoped<IMarketDataService, TinkoffMarketDataService>();
        services.AddSingleton<IRtsBoardInstrumentService, RtsBoardInstrumentService>();
        return services;
    }

    private static void ConfigureRtsBoardHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://rtsboard.ru", UriKind.Absolute);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json;  charset=utf-8");
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
    }
}