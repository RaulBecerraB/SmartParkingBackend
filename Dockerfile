FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SmartParkingBackend.csproj", "./"]
RUN dotnet restore "SmartParkingBackend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "SmartParkingBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartParkingBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Crear directorio para almacenar el .env 
RUN mkdir -p /app/data
# Establecer variable de entorno para indicar que estamos en producción
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SmartParkingBackend.dll"] 