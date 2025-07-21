using Application.Services;
using Domain.Interfaces;
using Domain.Interfaces.RtsBoard;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DI;

public static class ApplicationDi
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IInstrumentRepo, InstrumentRepo>();
        services.AddScoped<IQuotationService, QuotationService>();
        services.AddSingleton<IRtsBoardOpenDealsRepo, RtsBoardOpenDealsRepo>();
        services.AddSingleton<IMarketLevelRepo, MarketLevelRepo>();
        return services;
    }
}