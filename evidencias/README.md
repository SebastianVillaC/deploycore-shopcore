# Evidencias CI/CD

Capturas de cada etapa del flujo **GitHub → CI → Artefacto → CD → Azure**.

- `evidencias/ci/`: Integración Continua.
- `evidencias/cd/`: Entrega Continua y Azure.

## Cómo subir tus capturas

Las evidencias se suben **directo a `main`, sin pull request**. Los cambios dentro de `evidencias/` no disparan la CI ni el despliegue.

```bash
git checkout main
git pull origin main
# copia tu imagen a evidencias/ci/ o evidencias/cd/ con el nombre EXACTO de la tabla
git add evidencias/
git commit -m "docs(evidencias): <nombre de la captura>"
git push origin main
```

Si el `push` es rechazado porque alguien subió algo antes, ejecuta `git pull origin main` y repite el `git push`.

**Reglas**
- Formato **PNG** y el **nombre exacto** de la tabla.
- **Legible:** ventana maximizada, zoom del navegador al 100–125 %, sin recortar lo importante.
- 🔒 **Sin secretos:** en los App settings de Azure los valores deben verse **ocultos**. Nunca debe verse `jwtKey` ni `Seed__AdminPassword`.
- Puedes subir varias capturas en un mismo commit.
- Marca tu captura con ✅ en la columna **Estado** de esta tabla, en el mismo commit.

## CI (`evidencias/ci/`)

| # | Archivo | Qué muestra | Quién | Estado |
|---|---|---|---|---|
| 01 | `01_github_repo_main.png` | Repositorio en GitHub: rama `main` y colaborador `oalarconpe` (Settings → Collaborators) | Sebastian | ⬜ |
| 02 | `02_github_historial_commits.png` | Historial de commits y PR del equipo | Sebastian | ⬜ |
| 03 | `03_devops_service_connection_github.png` | Conexión de Azure DevOps con GitHub (app "Azure Pipelines" o service connection) | Eduardo | ⬜ |
| 04 | `04_devops_pipeline_yaml.png` | Pipeline `shopcore-ci` usando `pipelines/ci-shopcore.yml` | Eduardo | ⬜ |
| 05 | `05_ci_trigger_automatico.png` | Ejecución lanzada automáticamente por un merge a `main` (*Triggered by …* y commit) | Eduardo | ⬜ |
| 06 | `06_ci_etapas_ok.png` | Pasos de la ejecución en verde | Eduardo | ⬜ |
| 07 | `07_ci_pruebas.png` | Pestaña *Tests*: 10 pruebas aprobadas | Laura | ⬜ |
| 08 | `08_ci_artefactos.png` | Artefactos publicados: `shopcore-api` y `shopcore-web` | Eduardo | ⬜ |
| 09 | `09_github_pr_validacion.png` | Un pull request con el check de Azure Pipelines en verde | Laura | ⬜ |

## CD (`evidencias/cd/`)

| # | Archivo | Qué muestra | Quién | Estado |
|---|---|---|---|---|
| 01 | `01_azure_resource_group.png` | Grupo de recursos con el plan y los App Services `ShopCoreAPI` y `shopcoreweb` | David | ⬜ |
| 02 | `02_azure_app_settings_api.png` | App settings de `ShopCoreAPI` (**valores ocultos**) | David | ⬜ |
| 03 | `03_azure_runtime_net10.png` | Stack settings de `ShopCoreAPI` con .NET 10 | David | ⬜ |
| 04 | `04_devops_service_connection_azure.png` | Service connection de Azure Resource Manager | David | ⬜ |
| 05 | `05_release_artefacto_trigger.png` | Release "CD": artefacto `_shopcore-ci` y trigger de despliegue continuo con filtro `main` | Sebastian | ⬜ |
| 06 | `06_release_tareas.png` | Tareas del Release: despliegue de la API y del frontend | Sebastian | ⬜ |
| 07 | `07_release_automatico_ok.png` | Lista de releases: release creado automáticamente desde `main`, en verde | Sebastian | ⬜ |
| 08 | `08_release_logs_despliegue.png` | Logs de un release: despliegue de la API y del frontend exitosos | David | ⬜ |
| 09 | `09_api_health_publica.png` | `https://shopcoreapi-bgegbefuf2bnavbr.centralus-01.azurewebsites.net/health` **después** del cambio del video, con la versión y el commit del v1.1 | Laura | ⬜ |
| 10 | `10_web_antes_cambio.png` | Frontend **antes** del cambio del video (banner v1.0). Tomarla antes de grabar | Laura | ⬜ |
| 11 | `11_web_despues_cambio.png` | Frontend **después** del cambio del video (banner v1.1) | Brayan | ⬜ |

## Trazabilidad del cambio demostrado

El cambio controlado del video (v1.1) recorre todo el flujo: GitHub → CI → artefacto → Release → Azure. La tabla la completa **Laura** después del video.

| Cambio | Commit en `main` | Ejecución CI | Release | Versión en `/health` |
|---|---|---|---|---|
| v1.1 (video CI/CD) | `<<sha>>` | `<<shopcore-ci-…>>` | `<<Release-N>>` | `<<…>>` |
