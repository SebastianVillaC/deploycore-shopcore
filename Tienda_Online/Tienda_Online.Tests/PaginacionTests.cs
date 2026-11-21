using Tienda_Online.Backend.Helpers;
using Tienda_Online.Shared.DTOs;

namespace Tienda_Online.Tests;

public class PaginacionTests
{
    private static readonly IQueryable<int> Numeros = Enumerable.Range(1, 25).AsQueryable();

    [Fact]
    public void Paginate_PrimeraPagina_DevuelveLosPrimerosRegistros()
    {
        var resultado = Numeros.Paginate(new PaginacionDTO { Pagina = 1, NumeroRegistros = 10 }).ToList();

        Assert.Equal(Enumerable.Range(1, 10), resultado);
    }

    [Fact]
    public void Paginate_UltimaPagina_DevuelveSoloLosRestantes()
    {
        var resultado = Numeros.Paginate(new PaginacionDTO { Pagina = 3, NumeroRegistros = 10 }).ToList();

        Assert.Equal(new[] { 21, 22, 23, 24, 25 }, resultado);
    }

    [Fact]
    public void Paginate_PaginaFueraDeRango_DevuelveListaVacia()
    {
        var resultado = Numeros.Paginate(new PaginacionDTO { Pagina = 10, NumeroRegistros = 10 }).ToList();

        Assert.Empty(resultado);
    }
}
