using System.IO.Pipelines;
using System.Net.Sockets;

namespace Device.Simulator.Networking;

public sealed class DeviceConnectionContext : IDisposable
{
    public DeviceConnectionContext(NetworkStream stream, string deviceBarcode)
    {
        _networkStream = stream;
        Reader = PipeReader.Create(stream);
        Writer = PipeWriter.Create(stream);
        DeviceBarcode = deviceBarcode;

        _ = SimulateTemperatureAsync();
        _ = SimulateBatteryAsync();
        _ = SimulateSignalAsync();
    }

    private readonly NetworkStream _networkStream;

    public readonly AckMessageChanel AckMessageChanel = new();
    public PipeReader Reader { get; }
    public PipeWriter Writer { get; }
    public string DeviceBarcode { get; }

    public double Temperature { get; private set; } = Random.Shared.Next(20, 80);
    public byte Battery { get; private set; } = (byte)Random.Shared.Next(0, 100);
    public byte Strength { get; private set; } = (byte)Random.Shared.Next(10, 90);

    private CancellationTokenSource TokenSource { get; } = new();


    public bool IsHighTemperature => Temperature > 80;
    public bool IsLowBattery => Battery < 10;
    public bool IsWeakSignal => Strength < 15;

    private async Task SimulateTemperatureAsync()
    {
        // Oscillates between 15°C and 95°C so it crosses the 80° threshold
        // naturally during a test run, then comes back down.
        double direction = 1;

        while (!TokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(3), TokenSource.Token).ConfigureAwait(false);

            Temperature += direction * (Random.Shared.NextDouble() * 3 + 1);

            if (Temperature >= 95) direction = -1; // start cooling
            if (Temperature <= 15) direction = 1; // start heating
        }
    }

    private async Task SimulateBatteryAsync()
    {
        // Drains 1% every 30 seconds — reaches low battery (~9%) after ~45 minutes
        // Set DeviceConnectionDelaySec low in tests to trigger it faster
        while (!TokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), TokenSource.Token).ConfigureAwait(false);

            if (Battery > 0)
                Battery--;
        }
    }

    private async Task SimulateSignalAsync()
    {
        // Fluctuates between 10 and 100 — occasionally drops below 15 (alert threshold)
        while (!TokenSource.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), TokenSource.Token).ConfigureAwait(false);

            Strength = (byte)Random.Shared.Next(10, 100);
        }
    }

    public void Dispose()
    {
        TokenSource.Cancel();
        AckMessageChanel.Complete();
        _networkStream.Dispose();
    }
}