using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tienda_Online.Backend.Clases;
using Tienda_Online.Backend.Data;
using Tienda_Online.Shared.DTOs;
using Tienda_Online.Shared.Entidades;

namespace Tienda_Online.Tests;

public class ClsProductoTests
{
    private static DataContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"pruebas-{Guid.NewGuid()}")
            .Options;
        var context = new DataContext(options);
        for (var i = 1; i <= 15; i++)
        {
            context.Productos.Add(new Producto
            {
                Nombre = $"Producto {i:00}",
                Descripcion = "Producto de prueba",
                Precio = i * 1000,
                NombreFoto = "foto.png",
                Foto = "https://placehold.co/100"
            });
        }
        context.SaveChanges();
        return context;
    }

    private static clsProducto CrearServicio(DataContext context)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Cloudinary:CloudName"] = "pruebas" })
            .Build();
        return new clsProducto(context, config);
    }

    [Fact]
    public async Task ObtenerListaProducto_RespetaElTamanoDePagina()
    {
        using var context = CrearContexto();
        var servicio = CrearServicio(context);

        var resultado = await servicio.ObtenerListaProducto(new PaginacionDTO { Pagina = 1, NumeroRegistros = 12 });

        Assert.True(resultado.Exitoso);
        Assert.Equal(12, resultado.Respuesta!.Count());
    }

    [Fact]
    public async Task ObtenerTotalPaginas_CalculaPaginasRedondeandoHaciaArriba()
    {
        using var context = CrearContexto();
        var servicio = CrearServicio(context);

        var resultado = await servicio.ObtenerTotalPaginas(new PaginacionDTO { NumeroRegistros = 12 });

        Assert.True(resultado.Exitoso);
        Assert.Equal(2, resultado.Respuesta);
    }

    [Fact]
    public async Task CrearProducto_SinFoto_NoEsExitoso()
    {
        using var context = CrearContexto();
        var servicio = CrearServicio(context);

        var resultado = await servicio.CrearProducto(new Producto
        {
            Nombre = "Sin foto",
            Descripcion = "Debe fallar",
            Precio = 1,
            NombreFoto = "x.png",
            Foto = ""
        });

        Assert.False(resultado.Exitoso);
    }
}
