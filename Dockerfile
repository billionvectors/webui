# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["asimplevectorsWebUI/asimplevectorsWebUI.csproj", "./asimplevectorsWebUI/"]
COPY ["client_api/csharp/asimplevectors/asimplevectors.csproj", "./client_api/csharp/asimplevectors/"]
COPY ["certificate.pfx", "/app/certificate.pfx"]

# Restore dependencies
RUN dotnet restore "./asimplevectorsWebUI/asimplevectorsWebUI.csproj"

# Copy all files to the build context
COPY . .

# Build application
RUN dotnet build "./asimplevectorsWebUI/asimplevectorsWebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./asimplevectorsWebUI/asimplevectorsWebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["certificate.pfx", "/app/certificate.pfx"]

# Define the entry point
ENTRYPOINT ["dotnet", "asimplevectorsWebUI.dll"]
