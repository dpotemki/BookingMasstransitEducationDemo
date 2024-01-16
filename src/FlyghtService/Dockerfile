#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FlyghtService/FlyghtService.csproj", "FlyghtService/"]
COPY ["Contracts/FlyghtService.Contracts/FlyghtService.Contracts.csproj", "Contracts/FlyghtService.Contracts/"]
COPY ["Contracts/Shared.Contracts/Shared.Contracts.csproj", "Contracts/Shared.Contracts/"]
RUN dotnet restore "./FlyghtService/./FlyghtService.csproj"
COPY . .
WORKDIR "/src/FlyghtService"
RUN dotnet build "./FlyghtService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FlyghtService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyghtService.dll"]