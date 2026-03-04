using ClubId.Data;
using ClubId.Services; // IMPORTANTE: Agregá este using para que reconozca la carpeta Services
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN DE SERVICIOS ---

// 1. Configuración de Base de Datos
builder.Services.AddDbContext<LigabdContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection"));
});

// 2. Registro de tu nuevo Servicio de Imágenes (AQUÍ VA)
builder.Services.AddScoped<IImageService, ImageService>();

// 3. Configuración de Controladores y Vistas
builder.Services.AddControllersWithViews();

// --- FIN DE SECCIÓN DE SERVICIOS ---

var app = builder.Build();

// ... El resto de tu código (Middlewares y Rutas) queda igual ...

// Create DB schema
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LigabdContext>();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Jgrxeqp}/{action=Index}/{id?}");

app.Run();