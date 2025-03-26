#Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "SmartParkingBackend.csproj" --disable-parallel
RUN dotnet publish "SmartParkingBackend.csproj" -c release -o /app --no-restore

#Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-focal
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "SmartParkingBackend.dll"]


