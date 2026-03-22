using Gateway.Server.Extensions;
using Microsoft.Extensions.Hosting;

ThreadPool.GetMinThreads(out _, completionPortThreads: out int minIo);
ThreadPool.SetMinThreads(500, completionPortThreads: minIo);

var builder = Host.CreateApplicationBuilder(args);
builder.AddGatewayServer();
var app = builder.Build();
await app.RunAsync();