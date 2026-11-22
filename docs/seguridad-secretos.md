# Manejo de credenciales y secretos

Medidas aplicadas para no exponer credenciales en el repositorio:

1. **Repositorio con historial limpio.** El repositorio de la práctica anterior tenía claves en commits antiguos (Cloudinary, jwtKey, correo). Este repositorio se creó desde cero con el código ya limpio, así que esas claves no forman parte del historial. Además, se recomienda rotar las claves antiguas en sus proveedores.
2. **Sin secretos en `appsettings*.json`.** Los valores sensibles (`jwtKey`, `Seed:AdminPassword`, claves de Cloudinary) están vacíos en el repositorio.
3. **Secretos en Azure App Service.** En producción se definen como *App settings* del App Service (`jwtKey`, `Seed__AdminPassword`). Azure los inyecta como variables de entorno. Al reutilizar el App Service `ShopCoreAPI` de la práctica anterior, se **reemplazó su `jwtKey`** por una clave nueva.
4. **Secretos locales con User Secrets.** Cada desarrollador configura los suyos fuera del repositorio:
   ```bash
   dotnet user-secrets set jwtKey "<clave-de-32+-caracteres>" --project Tienda_Online/Tienda_Online.Backend
   ```
5. **La API falla al iniciar si falta un secreto.** Si `jwtKey` o `Seed:AdminPassword` no están configurados en producción, la API se detiene con un mensaje claro. Así nunca arranca con valores por defecto inseguros.
6. **Conexión a Azure administrada por Azure DevOps.** La service connection de tipo *Azure Resource Manager* guarda sus credenciales dentro de Azure DevOps: no hay client secret en la pipeline ni en el repositorio.
7. **Sin perfiles de publicación.** `.gitignore` excluye `*.pubxml` y `*.PublishSettings`, y no se usan perfiles de publicación con credenciales.
8. **Valores de prueba explícitos.** Las pruebas automatizadas usan valores ficticios definidos en `ShopCoreApiFactory`, que no sirven en producción.

Revisión antes de cada entrega:
```bash
git grep -n -i -E "password|secret|apikey|jwtKey" -- ':!docs' ':!*.md'
```
