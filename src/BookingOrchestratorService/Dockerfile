#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookingOrchestratorService/BookingOrchestratorService.csproj", "BookingOrchestratorService/"]
COPY ["Contracts/FlyghtService.Contracts/FlyghtService.Contracts.csproj", "Contracts/FlyghtService.Contracts/"]
COPY ["Contracts/HotelService.Contracts/HotelService.Contracts.csproj", "Contracts/HotelService.Contracts/"]
COPY ["Contracts/Shared.Contracts/Shared.Contracts.csproj", "Contracts/Shared.Contracts/"]
COPY ["Contracts/TransferService.Contracts/TransferService.Contracts.csproj", "Contracts/TransferService.Contracts/"]
COPY ["Contracts/UI.Contracts/UI.Contracts.csproj", "Contracts/UI.Contracts/"]
RUN dotnet restore "./BookingOrchestratorService/./BookingOrchestratorService.csproj"
COPY . .
WORKDIR "/src/BookingOrchestratorService"
RUN dotnet build "./BookingOrchestratorService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookingOrchestratorService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingOrchestratorService.dll"]