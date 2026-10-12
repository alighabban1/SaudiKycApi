# 1. بيئة التشغيل (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. بيئة البناء (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SaudiKycApi.csproj", "./"]
RUN dotnet restore "SaudiKycApi.csproj"
COPY . .
RUN dotnet build "SaudiKycApi.csproj" -c Release -o /app/build

# 3. النشر (Publish)
FROM build AS publish
RUN dotnet publish "SaudiKycApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. تشغيل التطبيق
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaudiKycApi.dll"]