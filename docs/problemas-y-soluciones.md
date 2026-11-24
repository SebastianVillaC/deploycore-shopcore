# Problemas encontrados y soluciones

Incidencias que encontró el equipo durante la práctica, su causa y cómo se resolvieron.

## Problema principal: migración de .NET 8 a .NET 10

La aplicación ShopCore estaba hecha en **.NET 8**. Durante la práctica, el **10 de noviembre de 2026**, terminó el soporte de .NET 8 (fin de su ciclo de vida LTS), y el App Service de Azure dejó de ofrecerlo como runtime para la aplicación. Por eso no se podía seguir desplegando con esa versión.

**Solución:** migrar todo el proyecto a **.NET 10** (LTS):
- `TargetFramework` cambiado a `net10.0` en los 4 proyectos (Backend, Frontend, Shared y Tests);
- SDK fijado en `global.json` (10.0.402). La pipeline de CI instala esa versión con `UseDotNet@2`;
- App Service `ShopCoreAPI` configurado con runtime **.NET 10**.

**Efecto secundario:** después de cambiar el framework, las **pruebas de integración empezaron a fallar con HTTP 500**. Los paquetes NuGet de Microsoft seguían en la versión 8.0.x, y el servidor de pruebas de `Microsoft.AspNetCore.Mvc.Testing` 8 no implementa `PipeWriter.UnflushedBytes`, que `System.Text.Json` de .NET 10 necesita. Se resolvió **alineando todos los paquetes de Microsoft a la versión 10.0.15** (EF Core, Identity, JwtBearer, Blazor WebAssembly y Mvc.Testing). Con eso las 10 pruebas volvieron a pasar en local y en la CI.

**Responsables:** David Morales Vargas (migración a .NET 10) y Jhon Eduardo Zabala Garzón (alineación de paquetes). Ver los PR #4 y su commit `fix(deps): alinear paquetes Microsoft a 10.0.15 para .NET 10`.

## Otros problemas

| # | Problema | Causa | Solución | Responsable |
|---|---|---|---|---|
| 1 | El historial del repositorio de la práctica anterior tenía claves (Cloudinary, jwtKey, correo) | Se versionaron en el primer commit de aquel repositorio | Repositorio nuevo con historial limpio. Los secretos van solo en App settings de Azure y en User Secrets (ver `seguridad-secretos.md`) | Sebastián Villa Castillo |
| 2 | La API no arrancaba sin SQL Server | La práctica anterior dependía de LocalDB o Azure SQL | EF Core InMemory: sin cadena de conexión ni costo; datos de demostración en cada inicio | Brayan Alexis Arango Orrego |
| 3 | En Production el seed fallaba con `An item with the same key has already been added` | Con `Seed:AdminPassword` de menos de 6 caracteres, Identity rechazaba el usuario sin informarlo y luego fallaba la confirmación del correo | Se valida el resultado de `CreateAsync` y se lanza un error claro. Se documenta el mínimo de 6 caracteres | Brayan Alexis Arango Orrego / Laura Posada Taborda |
| 4 | El formulario para pedir los parallel jobs gratuitos de Azure Pipelines ya no existe ("This form has been retired") | Microsoft cambió el proceso: la cuota gratuita ahora se otorga al configurar la facturación | Organization settings → Billing: se vinculó la suscripción de Azure. Quedó 1 job Microsoft-hosted gratis (1.800 min/mes); los jobs pagados se dejaron en 0 | Sebastián Villa Castillo |
| 5 | El frontend apuntaba siempre a la API antigua | La URL de la API estaba escrita en `Program.cs` | `ApiUrl` por entorno en `wwwroot/appsettings.{Entorno}.json` | Sebastián Villa Castillo |
| 6 | Se desplegaban en Azure builds de pull requests sin mergear (releases desde `refs/pull/5/merge`) | El Release no tenía filtro de rama en el trigger de despliegue continuo y la versión por defecto del artefacto era la última de cualquier rama | Filtro **Include `main`** en el trigger y versión por defecto *Latest from a specific branch → `main`*. Los PR solo se validan con la CI | Sebastián Villa Castillo / David Morales Vargas |
| 7 | Las rutas del frontend Blazor (`/productos`) podían dar 404 al recargar la página | El frontend es una SPA: el servidor debe responder `index.html` para rutas que no son archivos | App Service **Windows**: el `web.config` que genera `dotnet publish` incluye la regla de reescritura para SPA | David Morales Vargas |
| 8 | `/api/productos` fallaría si falta `Cloudinary:CloudName` | `clsProducto` crea el cliente de Cloudinary en el constructor | `CloudName` en `appsettings.Production.json` (no es secreto). En las pruebas se inyecta un valor ficticio | Laura Posada Taborda |
