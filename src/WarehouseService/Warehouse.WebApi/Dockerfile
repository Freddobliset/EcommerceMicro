FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["NugetLocal/", "NugetLocal/"]

COPY ["src/WarehouseService/EcommerceMicro/EcommerceMicro.csproj", "src/WarehouseService/EcommerceMicro/"]
COPY ["src/WarehouseService/Warehouse.Business/Warehouse.Business.csproj", "src/WarehouseService/Warehouse.Business/"]
COPY ["src/WarehouseService/Warehouse.Repository/Warehouse.Repository.csproj", "src/WarehouseService/Warehouse.Repository/"]
COPY ["src/WarehouseService/Warehouse.Shared/Warehouse.Shared.csproj", "src/WarehouseService/Warehouse.Shared/"]
COPY ["src/WarehouseService/Warehouse.ClientHttp/Warehouse.ClientHttp.csproj", "src/WarehouseService/Warehouse.ClientHttp/"]

RUN dotnet restore "./src/WarehouseService/EcommerceMicro/EcommerceMicro.csproj" --configfile ./nuget.config

COPY . .
WORKDIR "/src/src/WarehouseService/EcommerceMicro"
RUN dotnet build "./EcommerceMicro.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EcommerceMicro.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceMicro.dll"]