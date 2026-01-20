# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY HousewarmingRegistry.API.csproj .
RUN dotnet restore HousewarmingRegistry.API.csproj

# Copy everything else and build
COPY . .
RUN dotnet build HousewarmingRegistry.API.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish HousewarmingRegistry.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .

# Set environment variable for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "HousewarmingRegistry.API.dll"]

