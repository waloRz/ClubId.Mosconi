# ClubId (ASP.NET Core MVC, .NET 8) - MySQL prototype

Prototipo para gestionar jugadores de fútbol, generar carnets PDF y exportar CSV.
Usa **MySQL** via Pomelo.EntityFrameworkCore.MySql (EF Core provider).

## Requisitos
- .NET 8 SDK
- MySQL Server (8.x recommended) running and accessible
- (Opcional) Visual Studio 2022 o VS Code

## Configurar conexión MySQL
Abrí `appsettings.json` y ajustá la cadena `MySqlConnection`, por ejemplo:
```
"MySqlConnection": "Server=localhost;Port=3306;Database=clubid;User=root;Password=mi_contraseña;"
```

## Ejecutar
1. Restaurar paquetes:
```bash
dotnet restore
```
2. Ejecutar:
```bash
dotnet run
```
La app usa `EnsureCreated()` y creará las tablas automáticamente en la base `clubid` si tiene permisos.

## Notas
- Si querés usar migraciones EF (recomendado para producción), instalá `dotnet-ef` y ejecutá:
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Endpoints útiles:
- `/Players`
- `/Players/Create`
- `/Players/ExportCsv`
- `/Players/CardPdf/{id}`
