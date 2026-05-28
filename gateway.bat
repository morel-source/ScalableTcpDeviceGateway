@echo off
setlocal EnableDelayedExpansion

:: ============================================================
:: Scalable TCP IoT Gateway — Main controller
:: Usage:
::   gateway local    — infra in Docker, .NET services via dotnet run
::   gateway docker   — everything in Docker (full profile)
::   gateway stop     — stop and clean up everything
::   gateway status   — show what is running
:: ============================================================

set COMMAND=%1

if /I "%COMMAND%"=="local"  goto :local
if /I "%COMMAND%"=="docker" goto :docker
if /I "%COMMAND%"=="stop"   goto :stop
if /I "%COMMAND%"=="status" goto :status
goto :help

:: ════════════════════════════════════════════════════════
:: LOCAL — infra in Docker, .NET services via dotnet run
:: ════════════════════════════════════════════════════════
:local
echo.
echo ============================================================
echo  MODE: LOCAL
echo  Infrastructure  ^> Docker  (Kafka, Postgres, monitoring)
echo  .NET services   ^> dotnet run  (Gateway, Processor, Worker)
echo  Simulator       ^> run manually when ready
echo ============================================================
echo.

echo [1/2] Starting infrastructure (Kafka, Postgres, Grafana, Loki, Prometheus)...
docker-compose --profile infra up -d
if errorlevel 1 (
    echo ERROR: Failed to start infrastructure. Is Docker running?
    exit /b 1
)

echo.
echo [2/2] Waiting 20s for Kafka to be ready...
timeout /t 20 /nobreak >nul

echo.
echo ============================================================
echo  Infrastructure is up. Starting .NET services...
echo  Each service opens in its own window.
echo ============================================================
echo.

echo Starting Gateway.Server...
start "Gateway Server" cmd /k "dotnet run --project Gateway.Server/Gateway.Server.csproj"

timeout /t 3 /nobreak >nul

echo Starting TelemetryProcessor...
start "Telemetry Processor" cmd /k "dotnet run --project TelemetryProcessor/TelemetryProcessor.csproj"

timeout /t 2 /nobreak >nul

echo Starting AlertWorker...
start "Alert Worker" cmd /k "dotnet run --project AlertWorker/AlertWorker.csproj"

echo.
echo ============================================================
echo  All services started.
echo.
echo  Gateway TCP     : localhost:8888
echo  Kafka           : localhost:9092
echo  Kafka UI        : http://localhost:8080
echo  Prometheus      : http://localhost:9090
echo  Grafana         : http://localhost:3000  (admin / admin)
echo  PostgreSQL      : localhost:5432
echo.
echo  Run the simulator:
echo    dotnet run --project Device.Simulator/Device.Simulator.csproj
echo ============================================================
goto :eof

:: ════════════════════════════════════════════════════════
:: DOCKER — everything in Docker (full profile)
:: ════════════════════════════════════════════════════════
:docker
echo.
echo ============================================================
echo  MODE: DOCKER
echo  All services ^> Docker Compose  (full profile)
echo  Simulator      ^> run manually when ready
echo ============================================================
echo.

echo [1/2] Building .NET service images...
docker-compose --profile full build
if errorlevel 1 (
    echo ERROR: Build failed. Check your Dockerfiles.
    exit /b 1
)

echo.
echo [2/2] Starting all services...
docker-compose --profile full up -d
if errorlevel 1 (
    echo ERROR: Failed to start services.
    exit /b 1
)

echo.
echo Waiting 25s for all services to be healthy...
timeout /t 25 /nobreak >nul

echo.
echo ============================================================
echo  All services running in Docker.
echo.
echo  Gateway TCP     : localhost:8888
echo  Kafka           : localhost:9092
echo  Kafka UI        : http://localhost:8080
echo  Prometheus      : http://localhost:9090
echo  Grafana         : http://localhost:3000  (admin / admin)
echo  PostgreSQL      : localhost:5432
echo.
echo  Run the simulator:
echo    dotnet run --project Device.Simulator/Device.Simulator.csproj
echo ============================================================
goto :eof

:: ════════════════════════════════════════════════════════
:: STOP — tear everything down
:: ════════════════════════════════════════════════════════
:stop
echo.
echo Stopping all Docker services...
docker-compose --profile full --profile infra down
echo.
echo Closing any open dotnet run windows...
taskkill /FI "WINDOWTITLE eq Gateway Server*"       /F >nul 2>&1
taskkill /FI "WINDOWTITLE eq Telemetry Processor*"  /F >nul 2>&1
taskkill /FI "WINDOWTITLE eq Alert Worker*"         /F >nul 2>&1
echo.
echo All services stopped.
goto :eof

:: ════════════════════════════════════════════════════════
:: STATUS — show what is running
:: ════════════════════════════════════════════════════════
:status
echo.
echo ============================================================
echo  Running containers:
echo ============================================================
docker-compose --profile full --profile infra ps
goto :eof

:: ════════════════════════════════════════════════════════
:: HELP
:: ════════════════════════════════════════════════════════
:help
echo.
echo ============================================================
echo  Scalable TCP IoT Gateway — Controller
echo ============================================================
echo.
echo  Usage:  gateway [command]
echo.
echo  Commands:
echo.
echo    local    Infrastructure in Docker, .NET services via dotnet run.
echo             Best for development and debugging.
echo.
echo    docker   Everything in Docker (full profile).
echo             Best for testing the full stack end to end.
echo.
echo    stop     Stop all Docker containers and close dotnet windows.
echo.
echo    status   Show running containers.
echo.
echo  The simulator always runs outside Docker:
echo    dotnet run --project Device.Simulator/Device.Simulator.csproj
echo.
echo ============================================================
goto :eof