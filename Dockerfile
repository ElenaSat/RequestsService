FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5290

# Configuramos la URL para evitar el puerto 8080 por defecto de .NET 8
ENV ASPNETCORE_URLS=http://+:5290
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["RequestsService.Api/RequestsService.Api.csproj", "RequestsService.Api/"]
COPY ["RequestsService.Application/RequestsService.Application.csproj", "RequestsService.Application/"]
COPY ["RequestsService.Domain/RequestsService.Domain.csproj", "RequestsService.Domain/"]
COPY ["RequestsService.Infrastructure/RequestsService.Infrastructure.csproj", "RequestsService.Infrastructure/"]
RUN dotnet restore "./RequestsService.Api/RequestsService.Api.csproj"
COPY . .
WORKDIR "/src/RequestsService.Api"
RUN dotnet build "./RequestsService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./RequestsService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RequestsService.Api.dll"]
