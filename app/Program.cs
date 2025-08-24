using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using app.Data;
using app.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar servicios personalizados
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IDataSeederService, DataSeederService>();

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Configurar autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "Administrador"));
    options.AddPolicy("RRHHOrAdmin", policy => policy.RequireClaim("Role", "RRHH", "Administrador"));
});

// Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicializar datos de prueba si es necesario
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var seeder = services.GetRequiredService<IDataSeederService>();
        
        // Verificar si la base de datos existe y tiene tablas
        await context.Database.EnsureCreatedAsync();
        
        // Crear datos de prueba si no existen
        if (!await seeder.HasDataAsync())
        {
            await seeder.SeedAsync();
            Console.WriteLine("✅ Datos de prueba inicializados correctamente");
        }
        else
        {
            Console.WriteLine("ℹ️  La base de datos ya contiene datos");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al inicializar datos: {ex.Message}");
        // En producción, podrías querer manejar esto de manera diferente
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Middleware de autenticación y autorización
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Configurar rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ruta adicional para cuenta
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}",
    defaults: new { controller = "Account" });

app.Run();
