# ShopCore · Deploy Core

Tienda en línea **ShopCore** publicada con un flujo automatizado de **Integración Continua (CI)** y **Entrega Continua (CD)** usando **GitHub**, **Azure DevOps** y **Azure App Service**.

> Práctica final · Despliegue de Software · 2026

| | Enlace |
|---|---|
| 🌐 Frontend publicado | https://shopcoreweb-g4eqcddtajh2gph5.centralus-01.azurewebsites.net |
| 🔌 API publicada (Swagger) | https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net/swagger |
| ❤️ Comprobación de salud | https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net/health |
| ⚙️ Ejecuciones de CI | https://dev.azure.com/deploycore/DeployCore-ShopCore/_build |
| 🚀 Releases (CD) | https://dev.azure.com/deploycore/DeployCore-ShopCore/_release |
| 🎬 Video CI/CD | https://drive.google.com/file/d/1FPBmwqRKC-DGNKAeFTvKTOOSWZdIaVHj/view?usp=sharing |

## Equipo Deploy Core

| Integrante | Rol | Contribuciones |
|---|---|---|
| Sebastián Villa Castillo | Gestión de GitHub, Azure DevOps y documentación | Creación del repositorio y acceso del docente, organización y proyecto de Azure DevOps (billing y usuarios), importación de la base, configuración del frontend por entorno y banner de versión, ajustes del Release (filtro `main`, despliegue del frontend), documentación de Azure y CD, README |
| Brayan Alexis Arango Orrego | Ajustes de la aplicación | EF Core InMemory, manejo de secretos en la API, datos demo, `/health` con versión y commit |
| Laura Posada Taborda | Pruebas, evidencias y verificación | Proyecto xUnit (pruebas unitarias y de integración), carpeta `/evidencias/`, registro de problemas y soluciones |
| Jhon Eduardo Zabala Garzón | Configuración de CI | Pipeline `pipelines/ci-shopcore.yml`, trigger en `main`, artefactos, versión de build, integración en vivo del cambio controlado v1.1 (PR #8) |
| David Morales Vargas | Configuración de CD y Azure | Migración a .NET 10, App Services en Azure, service connection con Azure, creación del Release (CD), evidencias de CD |

## Descripción de la aplicación

ShopCore es una tienda en línea con catálogo de productos, ofertas, carrito de compras, facturas e informes. Se reutiliza la aplicación de la práctica anterior, que se publicaba manualmente en IIS y Azure desde Visual Studio. En esta práctica se automatiza todo su ciclo de construcción y despliegue.

| Proyecto | Tipo | Descripción |
|---|---|---|
| `Tienda_Online.Backend` | ASP.NET Core 10 Web API | API REST con JWT, Identity, EF Core (InMemory) y Swagger |
| `Tienda_Online.Frontend` | Blazor WebAssembly (.NET 10) | Interfaz web que consume la API |
| `Tienda_Online.Shared` | Class library | Entidades y DTOs compartidos |
| `Tienda_Online.Tests` | xUnit | Pruebas unitarias y de integración |

## Tecnología

- .NET 10 (SDK fijado en `global.json`), C#, ASP.NET Core, Blazor WebAssembly
- Entity Framework Core InMemory: no se necesita un servidor de base de datos
- xUnit + `Microsoft.AspNetCore.Mvc.Testing`
- GitHub: repositorio y rama principal **`main`**
- Azure DevOps: CI en YAML, CD con Release pipeline clásico
- Azure App Service (Windows): API en .NET 10 (`ShopCoreAPI`) y frontend Blazor como sitio estático

## Flujo CI/CD

```
 commit en main ──► CI (Azure DevOps) ──► artefactos ──► CD (Release) ──► Azure App Service
   GitHub          restore · build ·     shopcore-api    trigger de        API  + Frontend
                   test · publish        shopcore-web    despliegue        /health muestra
                                                         continuo          versión y commit
```

1. **Disparador:** el equipo integra los cambios con **pull requests** hacia `main`.
   - Cada PR ejecuta la pipeline `shopcore-ci` como **validación** (build + pruebas) antes del merge. Esos builds no se despliegan.
   - Al hacer merge, el nuevo commit en `main` vuelve a ejecutar `shopcore-ci` y de ahí parte el despliegue.
   - Los cambios que solo tocan `docs/` o `evidencias/` no la disparan.
2. **CI** ([`pipelines/ci-shopcore.yml`](pipelines/ci-shopcore.yml)):
   1. instala el SDK de .NET 10;
   2. restaura las dependencias;
   3. escribe la versión de build y el SHA del commit en `appsettings.json`;
   4. compila en Release;
   5. ejecuta las pruebas (los resultados aparecen en la pestaña *Tests*);
   6. publica dos artefactos: **`shopcore-api`** y **`shopcore-web`**.
3. **CD** ([`pipelines/cd-release.md`](pipelines/cd-release.md)): el Release de CD tiene habilitado el *continuous deployment trigger* sobre el artefacto de `shopcore-ci` (rama `main`). Cada CI exitosa crea un release que:
   1. despliega el zip de la API;
   2. despliega el zip del frontend;
4. **Sin aprobaciones manuales:** decidimos no poner aprobaciones previas al despliegue para que el flujo sea completamente automático. Si se agregaran, el release quedaría en espera hasta que un aprobador lo autorice.

**Trazabilidad:** `https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net/health` muestra `version` (número de ejecución de CI) y `commit` (SHA de GitHub). El banner de inicio del frontend también los muestra. Así cualquier versión publicada se puede rastrear hasta su commit, su ejecución de CI y su release.

### Pruebas automatizadas

```bash
dotnet test Tienda_Online/Tienda_Online.sln
```

Las pruebas cubren la paginación, `clsProducto` (con EF InMemory) y pruebas de integración de `/health` y `/api/productos`, incluido el control de acceso sin token.

## Ejecución local

Requisitos: [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
cd Tienda_Online
dotnet restore Tienda_Online.sln
dotnet build Tienda_Online.sln

# Secreto local para firmar los JWT (una sola vez, no se guarda en el repo)
dotnet user-secrets set jwtKey "una-clave-local-de-al-menos-32-caracteres" --project Tienda_Online.Backend

# API → http://localhost:5088/swagger  ·  http://localhost:5088/health
dotnet run --project Tienda_Online.Backend --launch-profile http

# Frontend (otra terminal) → http://localhost:5018
dotnet run --project Tienda_Online.Frontend --launch-profile http
```

En Development, el usuario administrador inicial es `david@gmail.com` con contraseña `123456`. En Azure la contraseña se toma del App setting `Seed__AdminPassword`.

## Configuración general

| Entorno | API | Frontend (`ApiUrl`) |
|---|---|---|
| Development | User Secrets (`jwtKey`) | `http://localhost:5088` |
| Production (Azure) | App settings `jwtKey`, `Seed__AdminPassword` | `https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net` |

Más detalle:

- [`docs/configuracion-azure.md`](docs/configuracion-azure.md): recursos de Azure, App settings y service connections.
- [`docs/seguridad-secretos.md`](docs/seguridad-secretos.md): cómo evitamos exponer credenciales.
- [`docs/problemas-y-soluciones.md`](docs/problemas-y-soluciones.md): errores encontrados y cómo se resolvieron.
- [`evidencias/`](evidencias/): capturas de CI y CD.

## Estructura del repositorio

```
├─ Tienda_Online/          código (.sln, Backend, Frontend, Shared, Tests)
├─ pipelines/              ci-shopcore.yml · cd-release.md
├─ docs/                   configuración, seguridad, problemas y soluciones
├─ evidencias/             ci/ · cd/
└─ global.json             versión del SDK de .NET
```
