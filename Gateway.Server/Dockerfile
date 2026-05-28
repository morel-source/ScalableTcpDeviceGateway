FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gateway.Server/Gateway.Server.csproj", "Gateway.Server/"]
COPY ["Gateway.Monitoring/Gateway.Monitoring.csproj", "Gateway.Monitoring/"]
COPY ["Gateway.Protocol/Gateway.Protocol.csproj", "Gateway.Protocol/"]
COPY ["Kafka.Producer/Kafka.Producer.csproj", "Kafka.Producer/"]
COPY ["Kafka.Contracts/Kafka.Contracts.csproj", "Kafka.Contracts/"]
COPY ["Shared.Logging/Shared.Logging.csproj", "Shared.Logging/"]
RUN dotnet restore "Gateway.Server/Gateway.Server.csproj"
COPY . .
WORKDIR "/src/Gateway.Server"
RUN dotnet build "./Gateway.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gateway.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Server.dll"]
