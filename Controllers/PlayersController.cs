using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClubId.Data;
using ClubId.Models;
using CsvHelper;
using System.Globalization;
using ClubId.Services;
//using SelectPdf;

namespace ClubId.Controllers
{
    public class PlayersController : Controller
    {
        private readonly  LigabdContext _context;   //ApplicationDbContext CONTEXTO 
        private readonly IWebHostEnvironment _env;

        public PlayersController(LigabdContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var players = from p in _context.Players select p;
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                players = players.Where(p =>
                    (p.FirstName != null && p.FirstName.Contains(q)) ||
                    (p.LastName != null && p.LastName.Contains(q)) ||
                    (p.Dni != null && p.Dni.Contains(q)) ||
                    (p.Team != null && p.Team.Contains(q))
                );
            }
            ViewBag.Query = q;
            return View(await players.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        public IActionResult Create() => View(new Player { BirthDate = DateTime.Today });

        [HttpPost]                                   // ENVIO DE DATOS A LA BASE DE DATOS
        [ValidateAntiForgeryToken]       // VALIDACIONES
        public async Task<IActionResult> Create(Player player, IFormFile? photo) //tomo la el objeto de la clase players y la foto para subirla a uploads
        {
            if (!ModelState.IsValid) return View(player);

            if (photo != null && photo.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var relPath = Path.Combine("uploads", fileName);
                var absPath = Path.Combine(_env.WebRootPath, relPath);
                Directory.CreateDirectory(Path.GetDirectoryName(absPath)!);
                using var stream = new FileStream(absPath, FileMode.Create);
                await photo.CopyToAsync(stream);
                player.PhotoPath = "/" + relPath.Replace('\\', '/' );
            }
           
            _context.Add(player);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Player input, IFormFile? photo)
        {
            if (id != input.Id) return NotFound();
            if (!ModelState.IsValid) return View(input);

            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.FirstName = input.FirstName;
            player.LastName = input.LastName;
            player.Dni = input.Dni;
            player.BirthDate = input.BirthDate;
            player.Team = input.Team;
            player.Number = input.Number;
            player.Active = input.Active;

            if (photo != null && photo.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var relPath = Path.Combine("uploads", fileName);
                var absPath = Path.Combine(_env.WebRootPath, relPath);
                Directory.CreateDirectory(Path.GetDirectoryName(absPath)!);
                using var stream = new FileStream(absPath, FileMode.Create);
                await photo.CopyToAsync(stream);
                player.PhotoPath = "/" + relPath.Replace('\\', '/')  ;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<FileResult> ExportCsv()
        {
            var players = await _context.Players.AsNoTracking().ToListAsync();
            using var ms = new MemoryStream();
            using (var writer = new StreamWriter(ms, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(players);
            }
            ms.Position = 0;
            return File(ms, "text/csv", "players.csv");
        }

      /*  public async Task<IActionResult> CardPdf(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            string photoHtml = string.IsNullOrWhiteSpace(player.PhotoPath)
                ? "<div style='width:80px;height:100px;background:#eee;border:1px solid #ccc'></div>"
                : $"<img src='{player.PhotoPath}' style='width:80px;height:100px;object-fit:cover;border:1px solid #ccc' />";

            string html = $@"
            <html>
            <head><meta charset='utf-8'></head>
            <body style='font-family:Arial'>
            <div style='border:2px solid #111;width:320px;height:200px;padding:10px;border-radius:8px;background:#f8f9ff'>
                <div style='display:flex;gap:10px'>
                <div>{photoHtml}</div>
                <div>
                    <h3 style='margin:0 0 6px 0'>CLUB ID · CREDENCIAL</h3>
                    <div><strong>{player.LastName.ToUpper()}, {player.FirstName}</strong></div>
                    <div>DNI: {player.Dni}</div>
                    <div>Equipo: {player.Team ?? "—"} · #{(player.Number?.ToString() ?? "—")}</div>
                    <div>F.Nac: {player.BirthDate:dd/MM/yyyy}</div>
                </div>
                </div>
                <div style='position:absolute;bottom:12px;right:16px;font-size:10px;color:#666'>
                Emitido: {DateTime.Today:dd/MM/yyyy}
                </div>
            </div>
            </body>
            </html>";

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MarginTop = 5;
            converter.Options.MarginBottom = 5;
            converter.Options.MarginLeft = 5;
            converter.Options.MarginRight = 5;
            PdfDocument doc = converter.ConvertHtmlString(html);
            var bytes = doc.Save();
            doc.Close();
            return File(bytes, "application/pdf", $"carnet_{player.LastName}_{player.FirstName}.pdf");
        }*/
    }
}
