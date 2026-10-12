FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copies everything including the subfolder
COPY . .

# Restores and publishes from inside the subfolder
WORKDIR /src/SaudiKycApi
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SaudiKycApi.dll"]
