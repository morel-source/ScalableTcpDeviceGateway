REM =========================================
REM This script choose which mode to run
REM =========================================

@echo off
REM Ensure the script knows its own location.
cd /d %~dp0

REM Check the command line argument (local, docker, or k8s).
if "%1"=="local" goto LOCAL
if "%1"=="docker" goto DOCKER
if "%1"=="k8s" goto K8S
if "%1"=="stop" goto STOP

REM If no valid argument was provided, show usage instructions.
echo =========================================
echo Usage:
echo   gateway local   - Run locally (dotnet)
echo   gateway docker  - Run full Docker stack
echo   gateway k8s     - Run Kubernetes mode
echo   gateway stop    - Stop everything
echo =========================================
exit /b 1

:LOCAL
echo [MODE] LOCAL
REM Run the local-specific script.
call scripts\run-local.bat
exit /b

:DOCKER
echo [MODE] DOCKER
REM Run the docker-specific script.
call scripts\run-docker.bat
exit /b

:K8S
echo [MODE] K8S
REM Run the kubernetes-specific script.
call scripts\run-k8s.bat
exit /b

:STOP
echo Stopping everything...
REM A safety command to shut down Docker and K8s at once.
docker compose down
kubectl delete -f k8s/ --ignore-not-found
echo Done.
exit /b
