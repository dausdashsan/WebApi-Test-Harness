# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["WebApiTestHarness.csproj", "./"]
RUN dotnet restore "WebApiTestHarness.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "WebApiTestHarness.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "WebApiTestHarness.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render provides a PORT environment variable.
# ASP.NET Core 8.0+ can be configured to listen on a specific port using ASPNETCORE_HTTP_PORTS.
# We'll default to 10000 which is common for Render, but it will be overridden if you set PORT in Render.
ENV ASPNETCORE_HTTP_PORTS=10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "WebApiTestHarness.dll"]
