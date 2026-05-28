using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Logging;
using Telemetry.Processor;
using Telemetry.Processor.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<TelemetryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(name: "Telemetry"))
);

builder.Services.AddHostedService<TelemetryProcessorWorker>();
builder.AddLogging(serviceName: "TelemetryProcessor");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();