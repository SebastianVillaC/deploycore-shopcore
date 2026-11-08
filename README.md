# ShopCore · Deploy Core

Tienda en línea **ShopCore** publicada con un flujo automatizado de **Integración Continua (CI)** y **Entrega Continua (CD)** usando **GitHub**, **Azure DevOps** y **Azure App Service**.

> Práctica final · Despliegue de Software · 2026

## Equipo Deploy Core

| Integrante | Rol | Responsabilidades |
|---|---|---|
| Integrante 1 (nombre completo) | Gestión de GitHub y documentación | Repositorio, rama `main`, organización de Azure DevOps, README, revisión del historial |
| Integrante 2 (nombre completo) | Ajustes de la aplicación | Configuración de la API para la nube, datos demo, health check |
| Integrante 3 (nombre completo) | Pruebas, evidencias y verificación | Pruebas automatizadas, carpeta `/evidencias/`, incidencias |
| Integrante 4 (nombre completo) | Configuración de CI | Pipeline YAML, ejecuciones, artefactos |
| Integrante 5 (nombre completo) | Configuración de CD y Azure | App Services, Release pipeline, validación final |

## Descripción de la aplicación

ShopCore es una tienda en línea con catálogo de productos, ofertas, carrito de compras, facturas e informes. Se reutiliza la aplicación de la práctica anterior (publicación manual en IIS y Azure desde Visual Studio). En esta práctica se automatiza todo su ciclo de construcción y despliegue.

| Proyecto | Tipo | Descripción |
|---|---|---|
| `Tienda_Online.Backend` | ASP.NET Core 8 Web API | API REST con JWT, Identity, EF Core y Swagger |
| `Tienda_Online.Frontend` | Blazor WebAssembly (.NET 8) | Interfaz web que consume la API |
| `Tienda_Online.Shared` | Class library | Entidades y DTOs compartidos |

## Tecnología

- .NET 8 (SDK 8.0.x), C#, ASP.NET Core, Blazor WebAssembly
- Entity Framework Core
- GitHub (repositorio, rama principal `main`)
- Azure DevOps Pipelines (CI en YAML, CD con Release pipeline)
- Azure App Service (Linux)

## Rama principal

Todo el trabajo se integra en **`main`**. Cada cambio confirmado en `main` dispara la pipeline de CI.

## Ejecución local

Requisitos: [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
cd Tienda_Online
dotnet restore Tienda_Online.sln
dotnet build Tienda_Online.sln

# API (http://localhost:5088/swagger)
dotnet run --project Tienda_Online.Backend --launch-profile http

# Frontend (en otra terminal)
dotnet run --project Tienda_Online.Frontend
```
