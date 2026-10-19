# Stage 1: build with the full SDK.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, so Docker can reuse this layer when only the code changes.
COPY TechServ.Api.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

# Stage 2: run with the smaller runtime image. It listens on port 8080 by default.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TechServ.Api.dll"]
