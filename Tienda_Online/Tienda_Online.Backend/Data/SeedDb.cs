using Microsoft.Identity.Client;
using Tienda_Online.Backend.Clases;
using Tienda_Online.Shared.Entidades;
using Tienda_Online.Shared.Enums;

namespace Tienda_Online.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly clsUsuario _usuarios;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public SeedDb(DataContext context, clsUsuario usuarios, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _usuarios = usuarios;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRoleAsync();
            await CheckUserAsync("Super", "Admin", "david@gmail.com", "3232942877", "Cr 23 8965", UserType.Administrador);
            await CheckProductosAsync();
        }

        //La base de datos InMemory arranca vacía en cada inicio, así que se cargan
        //productos de demostración para que el catálogo tenga contenido visible.
        private async Task CheckProductosAsync()
        {
            if (_context.Productos.Any())
            {
                return;
            }

            _context.Productos.AddRange(
                CrearProducto("Café de origen 500 g", "Café colombiano tostado medio, en grano.", 32000),
                CrearProducto("Chocolate artesanal", "Barra de chocolate 70 % cacao.", 12500),
                CrearProducto("Pan integral", "Pan integral de masa madre, 600 g.", 9800),
                CrearProducto("Queso campesino", "Queso fresco campesino, 500 g.", 15400),
                CrearProducto("Miel de abejas", "Miel pura de abejas, frasco de 350 g.", 18900),
                CrearProducto("Arepas de maíz", "Paquete de 10 arepas de maíz blanco.", 7600));
            await _context.SaveChangesAsync();
        }

        private static Producto CrearProducto(string nombre, string descripcion, double precio)
        {
            return new Producto
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                NombreFoto = $"{nombre}.png",
                Foto = $"https://placehold.co/500x500/png?text={Uri.EscapeDataString(nombre)}"
            };
        }

        //La contraseña del administrador inicial no se guarda en el código:
        //se lee de "Seed:AdminPassword" (App Setting "Seed__AdminPassword" en Azure).
        //Solo en Development existe un valor por defecto para facilitar las pruebas locales.
        private string GetAdminPassword()
        {
            var password = _configuration["Seed:AdminPassword"];
            if (!string.IsNullOrWhiteSpace(password))
            {
                return password;
            }
            if (_environment.IsDevelopment())
            {
                return "123456";
            }
            throw new InvalidOperationException(
                "Falta la configuración 'Seed:AdminPassword' (App Setting 'Seed__AdminPassword').");
        }

        public async Task CheckRoleAsync()
        {
            await _usuarios.CheckRoleAsync(UserType.Administrador.ToString());
            await _usuarios.CheckRoleAsync(UserType.Supervisor.ToString());
            await _usuarios.CheckRoleAsync(UserType.AsesorComercial.ToString());
            await _usuarios.CheckRoleAsync(UserType.Cliente.ToString());
        }

        private async Task<Usuario> CheckUserAsync(string nombre, string apellido, string email, string telefono, string direccion, UserType userType)
        {
            var user = await _usuarios.GetUserAsync(email);
            if(user == null)
            {
                user = new Usuario
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    PhoneNumber = telefono,
                    Direccion = direccion,
                    UserName = email,
                    userType = userType
                };
                var resultado = await _usuarios.AddUserAsync(user, GetAdminPassword());
                if (!resultado.Succeeded)
                {
                    //Sin esta validación, una contraseña que no cumple la política de Identity
                    //(mínimo 6 caracteres) hacía fallar el seed con un error poco claro.
                    throw new InvalidOperationException(
                        "No se pudo crear el administrador inicial: " +
                        string.Join("; ", resultado.Errors.Select(e => e.Description)));
                }
                await _usuarios.AddUserToRoleAsync(user, userType.ToString());
                var token =  await _usuarios.GenerateEmailConfirmationTokenAsync(user);
                await _usuarios.ConfirmEmailAsync(user, token);
            }
            return user;
        }
    }
}
