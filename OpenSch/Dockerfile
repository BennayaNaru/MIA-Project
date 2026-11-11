# syntax=docker/dockerfile:1

# The SPA is built separately so the .NET publish does not need Node installed.
FROM node:22-bookworm-slim AS spa
WORKDIR /src
COPY src/Web/ClientApp/package.json src/Web/ClientApp/package-lock.json ./
RUN npm ci
COPY src/Web/ClientApp/ ./
# Call Vite directly: the npm "build" script also regenerates the NSwag client from
# the .NET OpenAPI document, which is not available in this Node-only stage.
RUN npx vite build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so layer caching survives source-only changes.
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/Shared/Shared.csproj src/Shared/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/
COPY src/Web/Web.csproj src/Web/
RUN dotnet restore src/Web/Web.csproj

COPY . .
COPY --from=spa /src/build ./src/Web/wwwroot

RUN dotnet publish src/Web/Web.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false \
    /p:SkipClientBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Railway routes to the port the container listens on. Keep this in sync with the
# target port configured on the Railway domain.
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080

COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "OpenSch.Web.dll"]
