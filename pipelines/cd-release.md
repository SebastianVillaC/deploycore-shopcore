# CD · Release pipeline `CD`

La Entrega Continua se configuró como **Release pipeline clásico** en Azure DevOps, igual que en el material del docente. No se compila nada en el Release: solo se despliegan los artefactos publicados por la CI (`pipelines/ci-shopcore.yml`).

## Artefacto de origen

| Campo | Valor |
|---|---|
| Source type | Build |
| Source (build pipeline) | `shopcore-ci` |
| Default version | **Latest from a specific branch with tags → `main`** |
| Source alias | `_shopcore-ci` |
| Continuous deployment trigger | **Habilitado**, filtro de rama **Include `main`** |
| Pull request trigger | Deshabilitado |

- Con el trigger habilitado, **cada ejecución exitosa de la CI en `main` crea un release automáticamente**. No hace falta lanzarlo a mano.
- Las ejecuciones de CI de los pull requests (`refs/pull/N/merge`) **no se despliegan**: solo validan el PR. Ver el problema 10 en `docs/problemas-y-soluciones.md`.

## Stage `Stage 1`

Pre-deployment conditions: trigger *After release*, **sin aprobaciones**. El flujo es 100 % automático: merge en `main` → CI → Release → Azure.

| # | Tarea | Configuración |
|---|---|---|
| 1 | Azure App Service deploy (API) | Suscripción: Azure for Students · App type: *Web App on Windows* · App: `ShopCoreAPI` · Package: `$(System.DefaultWorkingDirectory)/_shopcore-ci/shopcore-api/*.zip` |
| 2 | Azure App Service deploy (Frontend) | Suscripción: Azure for Students · App type: *Web App on Windows* · App: `shopcoreweb` · Package: `$(System.DefaultWorkingDirectory)/_shopcore-ci/shopcore-web/*.zip` |
## Verificación del despliegue

Después de cada release se comprueba `https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net/health`:
- `"status": "OK"`;
- `"version"` igual al número de la ejecución de CI que generó el artefacto (el que muestra el release);
- `"commit"` igual al SHA del merge en `main`.

Así se demuestra que en Azure quedó publicado exactamente el artefacto de esa CI. El banner de la página de inicio del frontend muestra los mismos datos.

## Trazabilidad de un cambio

1. PR mergeado en `main` (GitHub, SHA `abc1234…`).
2. La CI `shopcore-ci-AAAAMMDD.N` se ejecuta automáticamente y publica `shopcore-api` y `shopcore-web`.
3. El Release `Release-N` se crea automáticamente con el artefacto `shopcore-ci-AAAAMMDD.N`.
4. `https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net/health` muestra `"version": "shopcore-ci-AAAAMMDD.N"` y `"commit": "abc1234…"`.
5. El banner de inicio del frontend muestra la versión visible y la versión/commit de la API.
