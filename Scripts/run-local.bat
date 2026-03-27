REM =======================================================================================
REM This script is designed for active development.
REM Running the app in Windows machine while keeping the heavy monitoring tools in Docker.
REM =======================================================================================

@echo off
REM Move to project root
cd /d %~dp0..

echo =========================================
echo Starting LOCAL mode...
echo =========================================

REM Check if Docker is running
docker info >nul 2>&1
REM If the previous command failed (Error Level not 0), stop the script.
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Docker Desktop is not running. 
    pause
    exit /b 1
)

echo Starting monitoring containers...
REM Start only Prometheus, Grafana, and Loki in the background.
docker compose up -d prometheus grafana loki

echo =========================================
echo Running Gateway...
echo Press Ctrl+C then press N to cleanup
echo =========================================

REM Enter the folder containing the C# source code.
cd Gateway.Server
REM Launch the .NET application and wait for it to close.
cmd /c dotnet run

echo.
echo Stopping monitoring containers...
REM Move back to the root folder to find the docker-compose file.
cd /d %~dp0..
REM Shut down and remove the monitoring containers.
docker compose down

echo Done.
pause