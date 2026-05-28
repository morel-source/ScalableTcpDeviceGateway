FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Telemetry.Processor/Telemetry.Processor.csproj", "Telemetry.Processor/"]
COPY ["Kafka.Contracts/Kafka.Contracts.csproj", "Kafka.Contracts/"]
COPY ["Gateway.Protocol/Gateway.Protocol.csproj", "Gateway.Protocol/"]
COPY ["Shared.Logging/Shared.Logging.csproj", "Shared.Logging/"]
RUN dotnet restore "Telemetry.Processor/Telemetry.Processor.csproj"
COPY . .
WORKDIR "/src/Telemetry.Processor"
RUN dotnet build "./Telemetry.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Telemetry.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telemetry.Processor.dll"]
