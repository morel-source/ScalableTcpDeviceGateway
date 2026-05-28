FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Alert.Worker/Alert.Worker.csproj", "Alert.Worker/"]
COPY ["Kafka.Contracts/Kafka.Contracts.csproj", "Kafka.Contracts/"]
COPY ["Gateway.Protocol/Gateway.Protocol.csproj", "Gateway.Protocol/"]
COPY ["Shared.Logging/Shared.Logging.csproj", "Shared.Logging/"]
RUN dotnet restore "Alert.Worker/Alert.Worker.csproj"
COPY . .
WORKDIR "/src/Alert.Worker"
RUN dotnet build "./Alert.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Alert.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alert.Worker.dll"]
