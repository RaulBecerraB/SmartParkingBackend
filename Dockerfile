# Usa la imagen base de SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los archivos del proyecto y restaura dependencias
COPY ["SmartParkingBackend.csproj", "."]
RUN dotnet restore "SmartParkingBackend.csproj"

# Copia todo el código y compila
COPY . .
RUN dotnet build "SmartParkingBackend.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "SmartParkingBackend.csproj" -c Release -o /app/publish

# Imagen final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Variables de entorno para la configuración de la aplicación
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Puerto que expone el contenedor
EXPOSE 80

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "SmartParkingBackend.dll"]

