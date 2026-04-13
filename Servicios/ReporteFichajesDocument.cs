using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models;

namespace ClubId.Services
{
    public class JugadorFichajeDto
{
    public string NombreCompleto { get; set; } = "";
    public string Dni { get; set; } = "";
    public string Equipo { get; set; } = "";
    public string Carnet { get; set; } = "";
    public DateTime FechaRecibo { get; set; }
}
    public class ReporteFichajesDocument : IDocument
    {
        private readonly List<JugadorFichajeDto> _datos;
        private readonly DateTime _desde;
        private readonly DateTime _hasta;
        private readonly string _webRootPath;

        public ReporteFichajesDocument(List<JugadorFichajeDto> datos, DateTime desde, DateTime hasta, string webRootPath)
        {
            _datos = datos;
            _desde = desde;
            _hasta = hasta;
            _webRootPath = webRootPath;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

 void ComposeHeader(IContainer container)
{
    var rutaAbsolutaLogo = Path.Combine(_webRootPath, "imgFijas", "escudoLiga.png");
    var rutaAbsolutaPelota = Path.Combine(_webRootPath, "imgFijas", "pelotaInf.webp");
    var colorInstitucional = "#0D6EFD"; 
    var colorLinea =   "#6a329f"; 

    container.Column(headerCol => 
    {
        headerCol.Item().Row(row =>
        {
            // Logo Izquierda
            row.RelativeItem(2).Column(c =>
            {
                if (System.IO.File.Exists(rutaAbsolutaLogo))
                    c.Item().Width(70).Image(rutaAbsolutaLogo);
                else
                    c.Item().Text("Logo no encontrado").FontSize(8).Italic();
            });

            // Centro: Títulos
            row.RelativeItem(6).Column(col =>
            {
                col.Item().AlignCenter().Text("ASOCIACION DEL NORTE DE VETERANOS").FontSize(14).Bold().FontColor("#A386C5");
                col.Item().AlignCenter().Text("Y SENIOR DE FUTBOL – SALTA").FontSize(12).Bold().FontColor("#A386C5");
                col.Item().AlignCenter().Text("REPORTE DE FICHAJES").FontSize(15).Bold().FontColor(colorInstitucional);
                col.Item().PaddingTop(5).AlignCenter().Text(DateTime.Now.ToString("dd 'de' MMMM 'del' yyyy")).FontSize(13).Bold().FontColor(Colors.Grey.Darken3);
                col.Item().AlignCenter().Text($"Periodo de Recibo: {_desde:dd/MM/yyyy} hasta {_hasta:dd/MM/yyyy}").FontSize(11);
            });

            // Logo Derecha
            row.RelativeItem(2).AlignRight().Column(c =>
            {
                if (System.IO.File.Exists(rutaAbsolutaPelota))
                    c.Item().Width(65).Image(rutaAbsolutaPelota);
                else
                    c.Item().Text("Pelota no encontrada").FontSize(8).Italic();
            });
        });

        // --- LINEA DIVISORIA DECORATIVA ---
        headerCol.Item().PaddingTop(10).LineHorizontal(2).LineColor(colorLinea);
    });
}

void ComposeContent(IContainer container)
{
    container.PaddingTop(10).Column(column =>
    {
        var gruposPorEquipo = _datos.GroupBy(x => x.Equipo);

        foreach (var grupo in gruposPorEquipo)
        {
            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30); 
                    columns.RelativeColumn(2);  
                    columns.RelativeColumn(5);  
                    columns.RelativeColumn(3);  
                    columns.RelativeColumn(2);  
                });

                table.Header(header =>
                {
                    header.Cell().ColumnSpan(5).Background(Colors.Blue.Lighten5).Padding(5).Row(row =>
                    {
                        row.RelativeItem().Text($"EQUIPO: {grupo.Key.ToUpper()}").FontSize(11).Bold().FontColor(Colors.Blue.Medium);
                        row.RelativeItem().AlignRight().Text($"{grupo.Count()} Jugadores").FontSize(9).Italic();
                    });

                    static IContainer HeaderStyle(IContainer container) => 
                        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(2);

                    header.Cell().Element(HeaderStyle).Text("#").Bold().FontSize(9);
                    header.Cell().Element(HeaderStyle).Text("Carnet").Bold().FontSize(9);
                    header.Cell().Element(HeaderStyle).Text("Nombre y Apellido").Bold().FontSize(9);
                    header.Cell().Element(HeaderStyle).Text("DNI").Bold().FontSize(9);
                    header.Cell().Element(HeaderStyle).Text("Fecha").Bold().FontSize(9);
                });

                int contador = 1;
                foreach (var item in grupo)
                {
                    static IContainer DataStyle(IContainer container) => 
                        container.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(3).AlignMiddle();

                    table.Cell().Element(DataStyle).Text(contador.ToString()).FontSize(9);
                    table.Cell().Element(DataStyle).Text(item.Carnet).FontSize(9);
                    table.Cell().Element(DataStyle).Text(item.NombreCompleto.ToUpper()).FontSize(9);
                    table.Cell().Element(DataStyle).Text(item.Dni).FontSize(9);
                    table.Cell().Element(DataStyle).AlignCenter().Text(item.FechaRecibo.ToString("dd/MM/yyyy")).FontSize(8);
                    contador++;
                }
            });
        }

        // --- RESUMEN FINAL DE JUGADORES ---
        column.Item().PaddingTop(30).Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn();
                cols.ConstantColumn(150);
            });

            table.Cell().AlignRight().PaddingRight(10).Text("TOTAL GENERAL DE JUGADORES PROCESADOS:").Bold().FontSize(12);
            table.Cell().Background(Colors.Blue.Medium).Padding(5).AlignCenter().Text($"{_datos.Count}").FontColor(Colors.White).Bold().FontSize(12);
        });
    });
}
        void ComposeFooter(IContainer container)
        {
            container.PaddingTop(10).Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Sistema ClubId - Listado de Control de Recibos - ").FontSize(8).Italic();
                        t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).Italic();
                    });

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Página ").FontSize(9);
                        x.CurrentPageNumber().FontSize(9).Bold();
                        x.Span(" de ").FontSize(9);
                        x.TotalPages().FontSize(9).Bold();
                    });
                });
            });
        }
    }
}