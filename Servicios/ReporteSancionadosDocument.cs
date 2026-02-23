using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models;

namespace ClubId.Services
{
    public class ReporteSancionadosDocument : IDocument
    {
        private readonly List<SancionReporteDto> _datos;
        private readonly DateTime _desde;
        private readonly DateTime _hasta;
        private readonly string _categoria;

        public ReporteSancionadosDocument(List<SancionReporteDto> datos, DateTime desde, DateTime hasta, string categoria)
        {
            _datos = datos;
            _desde = desde;
            _hasta = hasta;
            _categoria = categoria;
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
            container.Row(row =>
            {
                // Logo Izquierda (Placeholder)
                row.RelativeItem(2).Column(c =>
                {
                    if (System.IO.File.Exists("G:/Proyectos Web/ClubId_MySQL/Image/escudoLiga.png"))
                    {
                        c.Item().Image("G:/Proyectos Web/ClubId_MySQL/Image/escudoLiga.png");
                    }
                    else
                    {
                        // Si no encuentra la imagen, pone un texto para que no rompa el PDF
                        c.Item().Text("Logo no encontrado").FontSize(10).Italic();
                    }
                });

                // Centro: Títulos
                row.RelativeItem(6).Column(col =>
                {
                    col.Item().AlignCenter().Text("ASOCIACION DEL NORTE DE VETERANOS").FontSize(16).Bold().FontColor("#A386C5");
                    col.Item().AlignCenter().Text("Y SENIOR DE FUTBOL – SALTA").FontSize(14).Bold().FontColor("#A386C5");
                    col.Item().AlignCenter().Text("REPORTE SANCIONADOS").FontSize(16).Bold().FontColor("#A386C5");

                    col.Item().PaddingTop(5).AlignCenter().Text(DateTime.Now.ToString("'Martes' dd 'de' MMMM 'del' yyyy")).FontSize(14).Bold().FontColor(Colors.Red.Medium);

                    col.Item().AlignCenter().Text($"Reporte de Sancionados {_desde:dd/MM/yyyy} hasta {_hasta:dd/MM/yyyy}").FontSize(12);
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Categoría ").FontSize(13);
                        text.Span(_categoria).FontSize(15).Bold();
                    });
                });

                // Imagen Derecha (Tarjeta roja placeholder)
                row.RelativeItem(2).AlignRight().Text("🟥").FontSize(40);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Equipo
                    columns.RelativeColumn(3); // Nom y Ap
                    columns.RelativeColumn(2); // Carnet
                    columns.RelativeColumn(2); // Fecha
                    columns.RelativeColumn(3); // Sancion
                });

                table.Header(header =>
                {
                    // ✅ Cambiamos a la sintaxis de Span para aplicar Bold
                    header.Cell().Element(CellStyle).Text(t => t.Span("Equipo").Bold().FontColor(Colors.Red.Medium));
                    header.Cell().Element(CellStyle).Text(t => t.Span("Nombre y Apellido").Bold().FontColor(Colors.Red.Medium));
                    header.Cell().Element(CellStyle).Text(t => t.Span("Carnet").Bold().FontColor(Colors.Red.Medium));
                    header.Cell().Element(CellStyle).Text(t => t.Span("Fecha").Bold().FontColor(Colors.Red.Medium));
                    header.Cell().Element(CellStyle).Text(t => t.Span("Sancion").Bold().FontColor(Colors.Red.Medium));

                    static IContainer CellStyle(IContainer container) =>
                        container.Border(1).Padding(5).AlignCenter();
                });

                foreach (var item in _datos)
                {
                    // ✅ Usamos t.Span(...) para poder usar .Bold() sin errores
                    table.Cell().Element(DataCellStyle).Text(t => t.Span(item.Equipo).Bold().FontSize(11));
                    table.Cell().Element(DataCellStyle).Text(item.NombreCompleto.ToUpper()).FontSize(11);
                    table.Cell().Element(DataCellStyle).AlignCenter().Text(item.Carnet).FontSize(11);
                    table.Cell().Element(DataCellStyle).AlignCenter().Text($"F {item.NroFecha} – {item.FechaBoletin}").FontSize(11);
                    table.Cell().Element(DataCellStyle).Text(item.SancionTexto.ToUpper()).FontSize(11);

                    static IContainer DataCellStyle(IContainer container) =>
                        container.Border(1).Padding(5).AlignMiddle();
                }
            });
        }
    
    void ComposeFooter(IContainer container)
{
    container.PaddingTop(10).Column(col => 
    {
        // Línea divisoria superior
        col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
        
        col.Item().Row(row =>
        {
            // Texto a la izquierda (opcional, como el nombre del sistema)
            row.RelativeItem().Text(t => 
            {
                t.Span("Sistema ClubId - Generado el ").FontSize(9).Italic();
                t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).Italic();
            });

            // Numeración a la derecha
            row.RelativeItem().AlignRight().Text(x =>
            {
                x.Span("Página ").FontSize(10);
                x.CurrentPageNumber().FontSize(10).Bold(); // Número actual
                x.Span(" de ").FontSize(10);
                x.TotalPages().FontSize(10).Bold();     // Total de páginas
            });
        });
    });
}
    }

}