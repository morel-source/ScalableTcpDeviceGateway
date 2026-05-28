using Gateway.Server.Extensions;
using Kafka.Producer;
using Microsoft.Extensions.Hosting;
using Serilog;

ThreadPool.GetMinThreads(out _, completionPortThreads: out int minIo);
ThreadPool.SetMinThreads(500, completionPortThreads: minIo);

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddGatewayServer();
    builder.Services.AddKafkaProducer(builder.Configuration);
    var app = builder.Build();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}