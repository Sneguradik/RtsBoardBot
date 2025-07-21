
using Application.DI;
using Domain.Interfaces;
using Domain.Interfaces.RtsBoard;
using Infrastructure.DI;
using Infrastructure.Dto.RtsBoard;
using Quartz;

namespace Worker;

public static class ApplicationStartup
{
    public static HostApplicationBuilder AddConfigs(this HostApplicationBuilder builder)
    {
        builder.Services.Configure<RtsBoardLoginConfig>(
            builder.Configuration.GetSection("LoginConfig"));

        builder.Services.Configure<List<string>>(
            builder.Configuration.GetSection("Tickers"));

        return builder;
    }
    public static IServiceCollection AddConfiguredServices(this IServiceCollection services, string tinkoffToken)
    {
        services
            .AddApplicationServices()
            .AddInfrastructureServices(tinkoffToken);
        services.AddHostedService<MarketDataWorker>();
        return services;
    }
    
    public static IServiceCollection AddConfiguredQuartz(this IServiceCollection services, int pollingFrequencyInMinutes)
    {
        services.AddQuartzHostedService();
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(PosterJob));

            q.AddJob<PosterJob>(opts => opts
                .WithIdentity(jobKey)
                .WithDescription("Posts quotes or data periodically"));
            
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(nameof(PosterJob))
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromMinutes(pollingFrequencyInMinutes))
                    .RepeatForever()));
        });
        return services;
    }

    public static async Task PrepareApp(this IHost host, IEnumerable<string> tickers)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var marketLevelRepo = services.GetRequiredService<IMarketLevelRepo>();
        await marketLevelRepo.InitAsync(CancellationToken.None);

        var boardInstrumentService = services.GetRequiredService<IRtsBoardInstrumentService>();
        var boardInstrumentRepo =  services.GetRequiredService<IRtsBoardInstrumentRepo>();

        foreach (var instrument in (await boardInstrumentRepo
                     .GetAllInstruments())
                 .Where(x=>tickers.Contains(x.Id))) 
            boardInstrumentService.AddInstrument(instrument);
    }
}