FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["NugetLocal/", "NugetLocal/"]

COPY ["src/OrderService/Order.WebApi/Order.WebApi.csproj", "src/OrderService/Order.WebApi/"]
COPY ["src/OrderService/Order.Business/Order.Business.csproj", "src/OrderService/Order.Business/"]
COPY ["src/OrderService/Order.Repository/Order.Repository.csproj", "src/OrderService/Order.Repository/"]
COPY ["src/OrderService/Order.Shared/Order.Shared.csproj", "src/OrderService/Order.Shared/"]
COPY ["src/OrderService/Order.ClientHttp/Order.ClientHttp.csproj", "src/OrderService/Order.ClientHttp/"]

RUN dotnet restore "./src/OrderService/Order.WebApi/Order.WebApi.csproj" --configfile ./nuget.config

COPY . .
WORKDIR "/src/src/OrderService/Order.WebApi"
RUN dotnet build "./Order.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Order.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.WebApi.dll"]