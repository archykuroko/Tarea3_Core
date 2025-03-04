# Imagen base para tiempo de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Imagen para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tarea3_Core.csproj", "."]
RUN dotnet restore "./Tarea3_Core.csproj"
COPY . .
RUN dotnet build "./Tarea3_Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Imagen para publicar la aplicación
FROM build AS publish
RUN dotnet publish "./Tarea3_Core.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final para ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Tarea3_Core.dll"]
