REM =======================================================================
REM This script tests the app as a container.
REM Ensuring it works exactly like it would in a production environment.
REM =======================================================================

@echo off
REM Move to project root
cd /d %~dp0..

echo =========================================
echo Starting DOCKER mode...
echo =========================================

REM Check if Docker is running
docker info >nul 2>&1
REM If the previous command failed (Error Level not 0), stop the script.
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Docker Desktop is not running. 
    pause
    exit /b 1
)

echo Building Gateway image...
REM Compile the code into a Docker image named 'tcp-gateway'.
docker build -t tcp-gateway:latest .

echo Starting full container stack...
REM Start the App, Prometheus, Grafana, and Loki all at once.
docker compose up -d gateway-app prometheus grafana loki

echo =========================================
echo Docker stack is running
echo Press any key to stop everything
echo =========================================
REM Keep the window open so the containers stay alive
pause >nul

echo Stopping containers...
REM Shut down and remove the monitoring containers.
docker compose down

echo Done.
pause