using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tienda_Online.Shared.Entidades;

namespace Tienda_Online.Tests;

public class ApiIntegrationTests : IClassFixture<ShopCoreApiFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(ShopCoreApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_DevuelveOkConBaseDeDatosDisponible()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("OK", json.GetProperty("status").GetString());
        Assert.True(json.GetProperty("productos").GetInt32() > 0);
        Assert.True(json.TryGetProperty("version", out _));
        Assert.True(json.TryGetProperty("commit", out _));
    }

    [Fact]
    public async Task Productos_EsPublicoYDevuelveElCatalogoDemo()
    {
        var response = await _client.GetAsync("/api/productos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var productos = await response.Content.ReadFromJsonAsync<List<Producto>>();
        Assert.NotNull(productos);
        Assert.NotEmpty(productos);
    }

    [Fact]
    public async Task Productos_FiltroPorNombre_DevuelveSoloCoincidencias()
    {
        var productos = await _client.GetFromJsonAsync<List<Producto>>("/api/productos?Filtro=caf");

        Assert.NotNull(productos);
        Assert.All(productos, p => Assert.Contains("caf", p.Nombre, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CrearProducto_SinToken_DevuelveNoAutorizado()
    {
        var response = await _client.PostAsJsonAsync("/api/productos/CrearProducto", new { });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
