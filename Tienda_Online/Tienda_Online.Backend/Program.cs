using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tienda_Online.Backend.Clases;
using Tienda_Online.Backend.Data;
using Tienda_Online.Backend.Helpers;
using Tienda_Online.Shared.Entidades;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Base de datos: EF Core InMemory. No necesita SQL Server ni cadenas de conexión,
//así la API arranca igual en local, en la pipeline de CI y en Azure App Service.
//Los datos se recrean (seed) en cada inicio de la aplicación.
builder.Services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("ShopCore"));

//Inyectamos las clases de los servicios
builder.Services.AddScoped<clsProducto>();
builder.Services.AddScoped<clsPromocionProducto>();
builder.Services.AddScoped<clsCarritoCompra>();
builder.Services.AddScoped<clsFactura>();
builder.Services.AddScoped<clsInforme>();
builder.Services.AddScoped<clsUsuario>();
builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddIdentity<Usuario, IdentityRole>(x =>
{
    x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    x.SignIn.RequireConfirmedEmail = true;
    x.User.RequireUniqueEmail = false;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
    x.Password.RequiredLength = 6;
    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    x.Lockout.MaxFailedAccessAttempts = 5;
    x.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tienda_Online API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                        Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                        Example: 'Bearer 12345abcdef'<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
        new List<string>()
        }
    });
});

//La clave JWT es un secreto: nunca se guarda en el repositorio.
//Local: dotnet user-secrets set jwtKey "<clave>"  ·  Azure: App Setting "jwtKey".
var jwtKey = builder.Configuration["jwtKey"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Falta la configuración 'jwtKey' o tiene menos de 32 caracteres. " +
        "Local: dotnet user-secrets set jwtKey \"<clave>\" --project Tienda_Online.Backend. " +
        "Azure: agregar el App Setting 'jwtKey' en el App Service.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
    ClockSkew = TimeSpan.Zero
});


builder.Services.AddTransient<SeedDb>();

var app = builder.Build();



SeedData(app);

static void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
   .AllowAnyMethod()
   .AllowAnyHeader()
   .SetIsOriginAllowed(origin => true)
   .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

//Comprobación de salud usada por el smoke test del Release (CD).
//"version" y "commit" los escribe la pipeline de CI en appsettings.json antes de publicar,
//lo que permite relacionar lo desplegado con la ejecución de CI y el commit de GitHub.
app.MapGet("/health", async (DataContext db, IWebHostEnvironment env, IConfiguration config) =>
{
    var dbOk = await db.Database.CanConnectAsync();
    var body = new
    {
        status = dbOk ? "OK" : "DEGRADED",
        database = dbOk ? "OK (InMemory)" : "ERROR",
        productos = dbOk ? await db.Productos.CountAsync() : 0,
        environment = env.EnvironmentName,
        version = string.IsNullOrWhiteSpace(config["App:Version"]) ? "local" : config["App:Version"],
        commit = string.IsNullOrWhiteSpace(config["App:Commit"]) ? "local" : config["App:Commit"],
        utc = DateTime.UtcNow
    };
    return dbOk ? Results.Ok(body) : Results.Json(body, statusCode: 503);
}).AllowAnonymous();

app.MapControllers();

app.Run();

//Necesario para que el proyecto de pruebas use WebApplicationFactory<Program>.
public partial class Program { }
