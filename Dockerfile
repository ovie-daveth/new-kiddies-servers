# Kids Social Media Platform - Dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["ChatApp.Backend.csproj", "./"]
RUN dotnet restore "ChatApp.Backend.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "ChatApp.Backend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ChatApp.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p /app/wwwroot/uploads/images && \
    mkdir -p /app/wwwroot/uploads/videos && \
    mkdir -p /app/wwwroot/uploads/thumbnails

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "ChatApp.Backend.dll"]

