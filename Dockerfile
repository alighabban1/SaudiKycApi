FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ جميع الملفات
COPY . .

# الانتقال لمجلد المشروع وبناء مشروع الـ API المحدد فقط
WORKDIR /src/SaudiKycApi
RUN dotnet restore "SaudiKycApi.csproj"
RUN dotnet publish "SaudiKycApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# بيئة التشغيل النهائية
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SaudiKycApi.dll"]
