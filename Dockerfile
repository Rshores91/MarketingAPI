# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MarketingAPI.csproj", "./"]
RUN dotnet restore "MarketingAPI.csproj"
COPY . .
RUN dotnet build "MarketingAPI.csproj" -c Release -o /app/build
RUN dotnet publish "MarketingAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install Tailscale
RUN apt-get update && apt-get install -y \
    curl \
    iptables \
    && curl -fsSL https://tailscale.com/install.sh | sh \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy application
COPY --from=build /app/publish .

# Copy startup script
COPY start.sh /app/start.sh
RUN chmod +x /app/start.sh

EXPOSE 8080

ENTRYPOINT ["/app/start.sh"]