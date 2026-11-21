using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tienda_Online.Tests;

/// <summary>
/// Levanta la API en memoria para las pruebas de integración.
/// Los valores de configuración son solo para pruebas (no son secretos reales).
/// </summary>
public class ShopCoreApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("jwtKey", "clave-solo-para-pruebas-automatizadas-0123456789");
        builder.UseSetting("Seed:AdminPassword", "clave-pruebas");
        builder.UseSetting("Cloudinary:CloudName", "pruebas");
    }
}
