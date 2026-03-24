using Device.Simulator.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddDeviceSimulator();
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