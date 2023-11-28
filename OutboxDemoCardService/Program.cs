using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OutboxCardService;
using OutboxCardService.Repository;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder
            .AddConsole(options =>
            {
                options.Format = Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat.Systemd;
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
            })
            .SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<CardRepository>();
    })
    .Build();

host.Run();
