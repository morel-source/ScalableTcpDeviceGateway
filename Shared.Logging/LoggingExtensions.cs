using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace Shared.Logging;

public static class LoggingExtensions
{
    extension(HostApplicationBuilder builder)
    {
        public void AddLogging(string serviceName)
        {
            builder.AddSerilog(serviceName);
        }

        private void AddSerilog(string serviceName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                ).WriteTo.GrafanaLoki(
                    uri: builder.Configuration["LokiOptions:Url"] ?? "http://localhost:3100",
                    labels:
                    [
                        new LokiLabel { Key = "Application", Value = serviceName }
                    ],
                    propertiesAsLabels: ["level"],
                    textFormatter: new Serilog.Formatting.Display.MessageTemplateTextFormatter(
                        "{Message:lj}{NewLine}{Exception}"))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog();
        }
    }
}