# Smart Parking Backend

Este es el backend para el sistema de gestión de estacionamientos inteligentes.

## Requisitos

- [Docker](https://www.docker.com/products/docker-desktop/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Ejecución con Docker

### Entorno de Desarrollo

Para ejecutar el proyecto en modo desarrollo con hot-reload:

1. Clona el repositorio:

```bash
git clone <url-del-repositorio>
cd SmartParkingBackend
```

2. Inicia los servicios en modo desarrollo:

```bash
docker-compose -f docker-compose.dev.yml up
```

Esta configuración incluye:
- Hot-reload (los cambios en el código se aplican automáticamente)
- Se monta el directorio local del proyecto dentro del contenedor
- Configuración específica para desarrollo
- Swagger UI habilitado para pruebas de API

### Entorno de Producción

Para ejecutar el proyecto en modo producción:

```bash
docker-compose up -d
```

Esta instrucción iniciará:
- Un contenedor con SQL Server
- Un contenedor con la aplicación backend optimizada para producción

## Acceso a la API

La API estará disponible en:
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081
- Swagger UI: http://localhost:8080/swagger

## Base de datos

El proyecto utiliza SQL Server para almacenar los datos. Las migraciones se ejecutan automáticamente al iniciar la aplicación.

Detalles de la conexión (dentro de los contenedores):
- Servidor: sqlserver
- Base de datos: SmartParking
- Usuario: sa
- Contraseña: SmartParking123!

## Estructura del Proyecto

- `Controllers/`: Controladores API
- `Models/`: Modelos de datos y contexto EF Core
- `Repository/`: Repositorios para acceso a datos
- `Services/`: Servicios de negocio
- `DTOs/`: Objetos de transferencia de datos
- `Migrations/`: Migraciones de Entity Framework Core

## Desarrollo Local

Si deseas desarrollar localmente sin Docker:

1. Instala [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Configura SQL Server localmente
3. Actualiza el archivo `.env` con tu cadena de conexión
4. Ejecuta el proyecto:

```bash
dotnet run
```

## Debugging en Docker

Para hacer debugging dentro del contenedor de desarrollo:

1. En VS Code, instala la extensión "Docker"
2. Configura el archivo `launch.json` para conectarte al contenedor
3. Establece puntos de interrupción en tu código
4. Inicia la sesión de debugging 