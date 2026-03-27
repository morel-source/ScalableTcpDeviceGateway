REM ==============================================================================================
REM This script deploys the app to Kubernetes nodes while keeping the monitoring tools in Docker.
REM ==============================================================================================

@echo off
REM Move to project root
cd /d %~dp0..

echo =========================================
echo Starting KUBERNETES mode...
echo =========================================

REM Verify the Kubernetes cluster (Docker Desktop) is reachable
kubectl cluster-info >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Kubernetes is not running!
    echo Enable it in Docker Desktop.
    pause
    exit /b 1
)

echo Building Docker image...
REM Build the image so Kubernetes can use the latest code.
docker build -t tcp-gateway:latest .

echo Starting monitoring stack...
REM Start the monitoring tools in Docker (they stay outside K8s).
docker compose up -d prometheus grafana loki

echo Applying Kubernetes manifests...
REM Send the YAML files in the k8s/ folder to the cluster.
kubectl apply -f k8s/

echo =========================================
echo K8s stack is running (Gateway inside cluster)
echo Press any key to stop everything
echo =========================================
REM Wait for user input before cleaning up.
pause >nul

echo Cleaning Kubernetes...
REM Remove all pods/services defined in the k8s folder.
kubectl delete -f k8s/ --ignore-not-found

echo Stopping Docker containers...
REM Shut down and remove the monitoring containers.
docker compose down

echo Done.
pause