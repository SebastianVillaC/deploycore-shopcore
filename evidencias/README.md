# Evidencias CI/CD

Capturas de cada etapa del flujo **GitHub → CI → Artefacto → CD → Azure**.

## Integración Continua (`ci/`)

| # | Captura | Qué muestra |
|---|---|---|
| 01 | [01_github_repo_main.png](ci/01_github_repo_main.png) | Repositorio en GitHub: colaboradores del equipo y del docente (`oalarconpe`) |
| 02 | [02_github_historial_commits.png](ci/02_github_historial_commits.png) | Pull requests integrados en `main` por el equipo |
| 03 | [03_devops_service_connection_github.png](ci/03_devops_service_connection_github.png) | Conexión de Azure DevOps con GitHub |
| 04 | [04_devops_pipeline_yaml.png](ci/04_devops_pipeline_yaml.png) | Pipeline `shopcore-ci` definida en `pipelines/ci-shopcore.yml` |
| 05 | [05_ci_trigger_automatico.png](ci/05_ci_trigger_automatico.png) | Ejecución lanzada automáticamente por un cambio en `main` |
| 06 | [06_ci_etapas_ok.png](ci/06_ci_etapas_ok.png) | Pasos de la ejecución en verde |
| 07 | [07_ci_pruebas.png](ci/07_ci_pruebas.png) | Resultado de las pruebas automatizadas |
| 08 | [08_ci_artefactos.png](ci/08_ci_artefactos.png) | Artefactos publicados: `shopcore-api` y `shopcore-web` |
| 09 | [09_github_pr_validacion.png](ci/09_github_pr_validacion.png) | Pull request #8 validado por Azure Pipelines antes del merge |

## Entrega Continua y Azure (`cd/`)

| # | Captura | Qué muestra |
|---|---|---|
| 01 | [01_azure_resource_group.png](cd/01_azure_resource_group.png) | Grupo de recursos `shopcore` con el plan y los App Services |
| 02 | [02_azure_app_settings_api.png](cd/02_azure_app_settings_api.png) | App settings de `ShopCoreAPI` con los valores ocultos |
| 03 | [03_azure_runtime_net10.png](cd/03_azure_runtime_net10.png) | Runtime .NET 10 de `ShopCoreAPI` |
| 04 | [04_devops_service_connection_azure.png](cd/04_devops_service_connection_azure.png) | Service connection de Azure Resource Manager |
| 05 | [05_release_artefacto_trigger.png](cd/05_release_artefacto_trigger.png) | Release "CD": artefacto `_shopcore-ci` desde la rama `main` |
| 06 | [06_release_tareas.png](cd/06_release_tareas.png) | Tareas del Release: despliegue de la API y del frontend |
| 07 | [07_release_automatico_ok.png](cd/07_release_automatico_ok.png) | Releases creados automáticamente; Release-8 del cambio v1.1 en verde |
| 08 | [08_release_logs_despliegue.png](cd/08_release_logs_despliegue.png) | Historial de releases: despliegues desde `main` y, antes de configurar el filtro de rama, releases desde un pull request (`refs/pull/5/merge`) |
| 09 | [09_api_health_publica.png](cd/09_api_health_publica.png) | `/health` de la API publicada con la versión y el commit del cambio v1.1 |
| 10 | [10_web_antes_cambio.png](cd/10_web_antes_cambio.png) | Frontend publicado antes del cambio (banner v1.0) |
| 11 | [11_web_despues_cambio.png](cd/11_web_despues_cambio.png) | Frontend publicado después del cambio (banner v1.1) |

## Trazabilidad del cambio demostrado

El cambio controlado del video (v1.1) recorrió todo el flujo de forma automática: GitHub → CI → artefacto → Release → Azure.

| Cambio | Pull request | Commit en `main` | Ejecución CI | Release | Versión en `/health` |
|---|---|---|---|---|---|
| v1.1 · banner del frontend | #8 (`aceb6bf`) | `e6aefdb` | `shopcore-ci-20261124.2` | Release-8 | `shopcore-ci-20261124.2` · commit `e6aefdb` |
