# Scalable TCP Device Gateway

![Build Status](https://github.com/morel-source/ScalableTcpDeviceGateway/actions/workflows/dotnet.yml/badge.svg)

**Status:** Actively under development.

---

### 🚀 Overview

A high-performance IoT gateway capable of 10k+ concurrent connections, achieving near-zero allocation on the hot path
via .NET 10 and `System.IO.Pipelines`.

---

### ❓ Why this project?

In IoT and industrial automation, handling thousands of concurrent "chatty" devices is a common bottleneck. Traditional
`async/await` patterns over standard streams can lead to high GC pressure and memory fragmentation under extreme load.

This project demonstrates:

- **High-Performance Networking:** Engineering a zero-copy TCP provider using `System.IO.Pipelines` to eliminate GC
  overhead.
- **Cloud-Native Observability:** Implementing a "Golden Signals" monitoring stack with Prometheus, Grafana, and Loki.
- **Advanced Concurrency:** Managing thousands of stateful device sessions using thread-safe, low-overhead patterns.

---

### ✨ Key Features

- **High-Concurrency Engine:** Built with `System.IO.Pipelines` for non-blocking, zero-copy
  I/O.
- **Zero-Allocation Parsing:** Uses `ReadOnlySequence<byte>` to process streams without heap allocations.
- **Memory Efficiency:** Commands modeled as `readonly record structs` and `Span<T>` to keep data on the stack.
- **Full Observability:** Integrated Prometheus metrics, Grafana dashboards, and Loki logs for real-time monitoring.
- **Built-in Load Testing:** Custom `Device.Simulator` to stress-test backpressure and throughput.

---

### 🛠 Tech Stack

- **Framework:** .NET 10 (LTS)
- **Networking:** `System.IO.Pipelines` (High-performance, low-latency socket I/O)
- **Memory Management:** `ArrayPool<T>` and `Span<T>` for buffer management.
- **Observability:** Prometheus (Metrics), Grafana (Visualization), and Grafana Loki (Logging).
- **CI/CD:** GitHub Actions (Automated build & test).
- **Orchestration:** Docker Compose & Kubernetes (K8s).

---

### ⚡ Performance & Design Highlights

| Feature                 | Technical Implementation  | Impact                                           |
|:------------------------|:--------------------------|:-------------------------------------------------|
| Zero-Allocation Parsing | `ReadOnlySequence<byte>`  | Eliminates intermediate string/array copies.     |
| Backpressure            | `PipeReader / FlushAsync` | Pauses reading if the buffer exceeds capacity.   | 
| GC Pressure             | `readonly record structs` | Keeps data on the stack; minimizes object churn. | 
| I/O Efficiency          | `ValueTask`               | Reduces overhead for asynchronous operations.    | 

<details>
<summary><b>🚀 Benchmark Metrics (Zero-Allocation)</b></summary>

| Operation | Encoding | Decoding | Total Latency | Allocated |
|:----------|:---------|:---------|:--------------|:----------|
| Login     | 18.24 ns | 64.29 ns | 82.53 ns      | 0 B       |
| Heartbeat | 14.07 ns | 54.55 ns | 68.62 ns      | 0 B       |
| Ack       | 1.07 ns  | 36.71 ns | 37.78 ns      | 0 B       |

Note: Benchmarked using BenchmarkDotNet on the hot-path message loop
</details>

---

### 📦 Protocol Specification

Data is transmitted as a packed binary stream for maximum efficiency.

---

### 🏗 Architecture Diagram

```mermaid
    flowchart LR
    devices[Devices] -->|TCP Connections| gateway[TCP Gateway Server]
    gateway -->|Metrics| prometheus[Prometheus]
    prometheus --> grafana[Grafana]
    gateway -->|Logs| loki[Loki]
    loki --> grafana
```

---

### 📊 Observability & Monitoring

The system provides a "Single Pane of Glass" view into the gateway's health using Prometheus for telemetry and Grafana
Loki for distributed logging.

<details>
<summary><b>View Dashboard Screenshots</b></summary>

#### Real-time tracking of 10k+ active sessions and handshake latencies:

![Grafana Dashboard Screenshot](./Metrics/Images/device-gateway-simulator-metrics.png)

##### Metrics Exposed:

- `gateway_devices_expected_total`: The target number of devices configured for this simulation run.
- `gateway_active_connections`: Current established TCP sessions.
- `gateway_logins_total`: Total successful handshakes completed.
- `gateway_heartbeats_total`: Total heartbeats processed.
- `gateway_disconnects_total`: Count of socket closures, including both simulated drops and server-side disconnects.
- `gateway_login_duration_seconds`: Latency tracking for handshake sequences.
- `gateway_heartbeat_duration_seconds`: Tracks how long the server takes to respond to a heartbeat request.

#### Structured logs correlated with metric spikes for rapid debugging:

![Grafana Dashboard Screenshot](./Metrics/Images/device-gateway-logs.png)
![Grafana Dashboard Screenshot](./Metrics/Images/device-simulator-logs.png)

</details>

---

### 🔄 Connection Lifecycle

1. **Connect:** Device establishes a TCP socket.
2. **Login:** Device must send a valid `Login` message within a defined window to be registered.
3. **Stay Alive:** Device sends periodic `Heartbeat` messages to maintain the session.
4. **Disconnect:** Automatically handled when the socket is closed or a heartbeat timeout is triggered.

---

### 📂 Project Structure

```text

├── Gateway.Server/               # Core TCP engine (Pipelines & socket handling)
├── Gateway.Protocol/             # Protocol parsing & encoding
├── Gateway.Protocol.Tests/       # Unit tests for protocol logic
├── Gateway.Monitoring/           # Shared metrics & observability library
├── Device.Simulator/             # High-concurrency load testing tool
├── Benchmarks/                   # BenchmarkDotNet performance suites
├── k8s/                          # Kubernetes Manifests
│   ├── namespace.yaml            # Isolated environment setup
│   ├── deployment.yaml           # App scaling & container specs
│   └── service.yaml              # Networking & NodePort configuration
├── scripts/                      # Automation & Lifecycle
│   ├── run-local.bat             # Standalone development mode
│   ├── run-docker.bat            # Containerized environment mode
│   └── run-k8s.bat               # Kubernetes cluster deployment mode
├── Metrics/                      # Monitoring Configuration
│   ├── grafana/                  # Grafana provisioning & dashboards
│   ├── prometheus/               # Prometheus scrape configurations
│   ├── loki/                     # Loki log aggregation settings
│   └── images/                   # Screenshots for the README
├── gateway.bat                   # Main CLI controller (Menu)
├── docker-compose.yaml           # Local monitoring stack setup
├── Dockerfile                    # Multi-stage chiseled build
└── .dockerignore                 # Build optimization rules

```

---

### 🔄 Connection Lifecycle

1. **Connect:** Device establishes a TCP socket.
2. **Login:** Device must send a valid `Login` message within a defined window to be registered.
3. **Stay Alive:** Device sends periodic `Heartbeat` messages to maintain the session.
4. **Disconnect:** Automatically handled when the socket is closed or a heartbeat timeout is triggered.

---

### 📋 Prerequisites

- .NET 10 SDK (or later)
- Docker & Docker Compose

---

### 🚀 Deployment & Getting Started

This project is designed for architectural flexibility.
You can manage the entire lifecycle of the gateway using the main `gateway.bat` controller located in the root directory.

#### ⚡ Choose Your Environment

Instead of running individual scripts, use the gateway command followed by the mode you wish to trigger:

| Command        | Mode       | Best For...                 | What it does                                                |
|:---------------|:-----------|:----------------------------|:------------------------------------------------------------|
| gateway local  | Standalone | Quick code debugging        | Runs monitoring in Docker and the app via dotnet run.       |
| gateway docker | Container  | Environment consistency     | Builds a Chiseled image and runs the full stack in Compose. |
| gateway k8s    | Kubernetes | Scaling & High Availability | Deploys 2 replicas to a K8s namespace with NodePort access. |
| gateway stop   | Cleanup    | Resource management         | Stops all Docker containers and deletes K8s manifests.      |    

#### 🛠️ How to Use

1. **Open Terminal:** Open a command prompt in the project root.
2. **Start:** Type your chosen command and press Enter.
3. **Interact:** While the stack is active, connect simulators or view metrics via the configured ports.
4. **Stop & Clean:** When finished, press Ctrl+C or a key as prompted in the window. To ensure everything is fully
   cleared,
   you can run `gateway stop`.

--- 

### 🧪 Testing the Load

To simulate real-world device traffic, use the Load Simulator. This generates high-concurrency TCP connections to test the scaling capabilities of the Gateway.

```bash
dotnet run --project Device.Simulator/Device.Simulator.csproj
````

---

### 🚀 Ports Configuration

#### Local / Docker Mode

- Gateway TCP: `8888`
- Metrics: `2222`
- Prometheus: `9090`
- Grafana: `3000` (User: `admin` / Pass: `admin`)

#### Kubernetes Mode (NodePorts)

- Gateway TCP: `30888`
- Metrics: `30222`

  Note: When running in Kubernetes mode, external tools like the `Device.Simulator` must target the NodePort (`30888`) to reach the gateway pods.

---

### 🚧 Future Improvements

- TLS for secure device communication
- Horizontal scaling (multi-instance gateway)
- Kafka / RabbitMQ integration for downstream data processing.
- Advanced rate limiting & device throttling.



