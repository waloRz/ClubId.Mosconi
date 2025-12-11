using ClubId.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB: MySQL (Pomelo). For testing locally you can also switch to UseSqlite.
// Replace user/password/database/host as needed in appsettings.json.
var conn = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<LigabdContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Create DB schema if it doesn't exist (no migrations required to start)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LigabdContext>();
    db.Database.EnsureCreated();
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
