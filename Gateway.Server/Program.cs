using Gateway.Server.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;

ThreadPool.GetMinThreads(out _, completionPortThreads: out int minIo);
ThreadPool.SetMinThreads(500, completionPortThreads: minIo);

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddGatewayServer();
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