#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TransferService/TransferService.csproj", "TransferService/"]
COPY ["Contracts/Shared.Contracts/Shared.Contracts.csproj", "Contracts/Shared.Contracts/"]
COPY ["Contracts/TransferService.Contracts/TransferService.Contracts.csproj", "Contracts/TransferService.Contracts/"]
RUN dotnet restore "./TransferService/./TransferService.csproj"
COPY . .
WORKDIR "/src/TransferService"
RUN dotnet build "./TransferService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TransferService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransferService.dll"]