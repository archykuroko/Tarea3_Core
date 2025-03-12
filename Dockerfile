# Imagen base para tiempo de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Imagen para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=$BUILD_CONFIGURATION
WORKDIR /src

# Copiar solo el archivo de proyecto y restaurar dependencias
COPY ["Tarea3_Core.csproj", "./"]
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . .
RUN dotnet build "./Tarea3_Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Imagen para publicar la aplicación
FROM build AS publish
RUN dotnet publish "./Tarea3_Core.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final para ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exponer el puerto en la imagen final
EXPOSE 5000

# Configuración de la URL
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Tarea3_Core.dll"]
