using Device.Simulator.Extensions;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.AddDeviceSimulator();
var app = builder.Build();
await app.RunAsync();