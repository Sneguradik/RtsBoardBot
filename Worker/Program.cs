

using Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddConfigs();

var tinkToken = Environment.GetEnvironmentVariable("TinkoffToken")??throw new Exception("TinkoffToken not set");

builder.Services.AddConfiguredServices(
    tinkToken)
    .AddConfiguredQuartz(builder.Configuration.GetValue<int?>("PostingFrequencyInMinutes")??
                         throw new Exception("Posting frequency was not found"));

var tickers = builder.Configuration.GetSection("Tickers").Get<string[]>();

var host = builder.Build();
await host.PrepareApp(tickers);
host.Run();