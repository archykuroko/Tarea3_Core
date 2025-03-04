# Tarea3_Core


###  Descripción
Este proyecto es una aplicación en **ASP.NET Core con SQL Server**, diseñada para la autenticación de usuarios.  
Utiliza **Docker y Docker Compose** para facilitar la ejecución y despliegue.



##  1. Requisitos Previos
Antes de instalar y ejecutar la aplicación, asegúrate de tener los siguientes programas instalados:

### 🔧 Herramientas necesarias
- .NET 9 SDK → https://dotnet.microsoft.com/en-us/download
- SQL Server Management Studio (SSMS) (Opcional, para gestionar la base de datos) → https://aka.ms/ssmsfullsetup
- Docker Desktop → https://www.docker.com/products/docker-desktop/
- Git (Opcional, si deseas clonar el repositorio) → https://git-scm.com/downloads



##  2. Instalación y Configuración
###  Clonar el repositorio
Si aún no tienes el código, clónalo con:
git clone https://github.com/archykuroko/Tarea3_Core cd tu-repositorio


###  Configuración de la base de datos
1. **Asegúrate de que Docker esté ejecutándose.**
2. **Ejecuta el siguiente comando para iniciar SQL Server en Docker:**

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver_container -d mcr.microsoft.com/mssql/server:2022-latest

3. **Conéctate a SQL Server con SSMS o desde la terminal** usando:

   - **Servidor:** `localhost,1433`
   - **Usuario:** `sa`
   - **Contraseña:** `YourStrong!Passw0rd`

4. **Ejecuta las migraciones para crear la base de datos (si usas EF Core)**:

dotnet ef database update


---

## 3. Ejecutar la Aplicación en Local
Si deseas ejecutar la aplicación sin Docker:
dotnet run


Luego abre en tu navegador:
http://localhost:5000


---

## 🔹 4. Desplegar con Docker
Para ejecutar la aplicación con Docker, usa **Docker Compose**.

### 📦 Construir la imagen de la aplicación
docker-compose build


### 🚀 Levantar los contenedores (App + SQL Server)
docker-compose up -d


### 📌 Verificar los contenedores en ejecución
docker ps


---

## 🔹 5. Acceder a la Aplicación
Una vez levantados los contenedores, accede a:

http://localhost:5000





Si tienes problemas con la conexión a la base de datos, revisa la cadena de conexión en `appsettings.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver_container;Database=Tarea3_Core;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
}


