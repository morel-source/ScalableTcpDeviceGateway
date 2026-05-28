using Alert.Worker;
using Alert.Worker.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<DeviceStatusHandler>();
builder.Services.AddSingleton<DeadLetterHandler>();
builder.Services.AddHostedService<AlertWorker>();

builder.AddLogging(serviceName: "AlertWorker");

var app = builder.Build();
await app.RunAsync();