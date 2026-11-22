# Configuración de Azure App Service

## Recursos

| Recurso | Nombre | Configuración |
|---|---|---|
| Suscripción | Azure for Students | La service connection del Release apunta a esta suscripción |
| Resource group | `shopcore` | Región: Central US |
| App Service plan | `ASP-shopcore-b54b` | **Windows** |
| Web App (API) | `ShopCoreAPI` | Publish: Code · Runtime stack: **.NET 10** · OS: Windows |
| Web App (Frontend) | `shopcoreweb` | Publish: Code · Runtime stack: **.NET 10** · OS: Windows (sirve archivos estáticos) |

URLs públicas:

- API: `https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net`. Comprobación de salud en `/health`, documentación en `/swagger`.
- Frontend: `https://shopcoreweb-g4eqcddtajh2gph5.centralus-01.azurewebsites.net`

> **Reutilización del App Service:** `ShopCoreAPI` se creó en la práctica anterior, cuando se publicaba manualmente desde Visual Studio. En esta práctica se reutiliza como destino del CD. El **código** que corre ahí se reemplaza por completo en cada despliegue con el artefacto de la CI. La **configuración** del App Service (App settings y runtime) se revisó y se actualizó: ver la siguiente sección.

## API · `ShopCoreAPI`

**Settings → Environment variables → App settings**

| Nombre | Valor | ¿Secreto? |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | No |
| `jwtKey` | clave aleatoria **nueva** de 32 caracteres o más | **Sí**: solo vive en Azure |
| `Seed__AdminPassword` | contraseña del administrador inicial (mínimo 6 caracteres) | **Sí**: solo vive en Azure |

Ajustes hechos al reutilizar el App Service:
- **Se reemplazó `jwtKey`** por una clave nueva. La anterior pertenecía a la práctica anterior y había quedado expuesta en el historial de aquel repositorio.
- Se agregó `Seed__AdminPassword`. Sin este valor, la API se detiene al arrancar en Production.
- Se eliminó la cadena de conexión a la base de datos de la práctica anterior. La API ahora usa EF Core InMemory y los datos demo se recrean en cada inicio.
- *Configuration → Stack settings*: runtime **.NET 10**.

Otros puntos:
- `App:Version` y `App:Commit` **no** se configuran en Azure. Los escribe la CI dentro del artefacto.
- `Cloudinary:CloudName` viene de `appsettings.Production.json` (no es secreto). Las claves de Cloudinary no se configuran, así que la subida de imágenes queda deshabilitada en esta demo.

## Frontend · `shopcoreweb`

El frontend Blazor WebAssembly son archivos estáticos. En un App Service **Windows** los sirve IIS. El `web.config` que genera `dotnet publish` redirige las peticiones a `wwwroot` y resuelve las rutas de la SPA, de modo que `/productos` funciona al recargar. No hace falta startup command.

La URL de la API la toma de `wwwroot/appsettings.Production.json` (`ApiUrl`), que va dentro del artefacto.

## Conexiones de servicio (Azure DevOps → Project settings → Service connections)

| Nombre | Tipo | Uso |
|---|---|---|
| GitHub | GitHub App "Azure Pipelines" instalada en el repositorio | La CI lee el repositorio y recibe los eventos de push y PR |
| Azure | Azure Resource Manager · suscripción Azure for Students | El Release despliega en los App Services |
