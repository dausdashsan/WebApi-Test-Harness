# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["WebApiTestHarness.csproj", "./"]
RUN dotnet restore "WebApiTestHarness.csproj"

COPY . .
RUN dotnet publish "WebApiTestHarness.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render injects PORT at runtime; we read it in Program.cs via Environment.GetEnvironmentVariable("PORT")
# Default to 10000 if PORT is not set
ENV PORT=10000
EXPOSE 10000

HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:${PORT}/health || exit 1

ENTRYPOINT ["dotnet", "WebApiTestHarness.dll"]
