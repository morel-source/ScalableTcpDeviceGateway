# STAGE 1: BUILD
# Use the full .NET 10 SDK to compile the code.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Create a workspace folder inside the Docker container called 'src'.
WORKDIR /src

# Copy ONLY the project definition files (.csproj) first.
# This allows Docker to cache the 'restore' step if the dependencies haven't changed.
COPY ["Gateway.Server/Gateway.Server.csproj", "Gateway.Server/"]
COPY ["Gateway.Protocol/Gateway.Protocol.csproj", "Gateway.Protocol/"]
COPY ["Gateway.Monitoring/Gateway.Monitoring.csproj", "Gateway.Monitoring/"]

# Download all required NuGet libraries from the internet.
RUN dotnet restore "Gateway.Server/Gateway.Server.csproj"

# Copy every single source file from the computer into the container.
# This includes all the .cs files for the Gateway logic.
COPY . .

# Compile the code into a clean, ready-to-run folder called '/app/publish' 
# -c Release: Optimizes the code for performance.
# -o /app/publish: Sets the output folder.
# /p:UseAppHost=false: Ensures it use the generic .NET runtime launcher.
RUN dotnet publish "Gateway.Server/Gateway.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: RUNTIME
# Switch to a "chiseled" runtime image. 
# This version has no shell or extra tools, making it extremely small and secure.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled

# Create a fresh folder called 'app' for the final execution.
WORKDIR /app

# Grab ONLY the compiled binaries from the 'build' stage and put them here. 
# This leaves the heavy SDK and source code behind.
COPY --from=build /app/publish .

# Define the command that runs when the container starts. 
# This launches the Gateway.Server using the .NET runtime.
ENTRYPOINT ["dotnet", "Gateway.Server.dll"]