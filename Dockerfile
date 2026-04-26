# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["FederationPlatform.sln", "./"]
COPY ["src/FederationPlatform.Domain/FederationPlatform.Domain.csproj", "src/FederationPlatform.Domain/"]
COPY ["src/FederationPlatform.Application/FederationPlatform.Application.csproj", "src/FederationPlatform.Application/"]
COPY ["src/FederationPlatform.Infrastructure/FederationPlatform.Infrastructure.csproj", "src/FederationPlatform.Infrastructure/"]
COPY ["src/FederationPlatform.Web/FederationPlatform.Web.csproj", "src/FederationPlatform.Web/"]

# Restore dependencies
RUN dotnet restore "FederationPlatform.sln"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/FederationPlatform.Web"
RUN dotnet build "FederationPlatform.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FederationPlatform.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install necessary packages for Persian culture support
RUN apt-get update && apt-get install -y \
    locales \
    && rm -rf /var/lib/apt/lists/*

# Set up Persian locale
RUN sed -i '/fa_IR.UTF-8/s/^# //g' /etc/locale.gen && \
    locale-gen

ENV LANG=fa_IR.UTF-8
ENV LANGUAGE=fa_IR:fa
ENV LC_ALL=fa_IR.UTF-8

# Copy published files
COPY --from=publish /app/publish .

# Create directories for uploads and logs
RUN mkdir -p /app/wwwroot/uploads/activities && \
    mkdir -p /app/wwwroot/uploads/profiles && \
    mkdir -p /app/wwwroot/uploads/news && \
    mkdir -p /app/Logs && \
    chmod -R 755 /app/wwwroot/uploads && \
    chmod -R 755 /app/Logs

# Expose port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FederationPlatform.Web.dll"]
