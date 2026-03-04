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
        private readonly string _webRootPath; // Para la ruta física de las imágenes

        public ReporteSancionadosDocument(List<SancionReporteDto> datos, DateTime desde, DateTime hasta, string categoria, string webRootPath)
        {
            _datos = datos;
            _desde = desde;
            _hasta = hasta;
            _categoria = categoria;
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
            // Construimos la ruta física absoluta
            var rutaAbsolutaLogo = Path.Combine(_webRootPath, "imgFijas", "escudoLiga.png");

            container.Row(row =>
            {
                // Logo Izquierda
                row.RelativeItem(2).Column(c =>
                {
                    if (System.IO.File.Exists(rutaAbsolutaLogo))
                    {
                        c.Item().Width(70).Image(rutaAbsolutaLogo);
                    }
                    else
                    {
                        c.Item().Text("Logo no encontrado").FontSize(8).Italic();
                    }
                });

                // Centro: Títulos
                row.RelativeItem(6).Column(col =>
                {
                    col.Item().AlignCenter().Text("ASOCIACION DEL NORTE DE VETERANOS").FontSize(14).Bold().FontColor("#A386C5");
                    col.Item().AlignCenter().Text("Y SENIOR DE FUTBOL – SALTA").FontSize(12).Bold().FontColor("#A386C5");
                    col.Item().AlignCenter().Text("REPORTE SANCIONADOS").FontSize(15).Bold().FontColor("#A386C5");

                    col.Item().PaddingTop(5).AlignCenter().Text(DateTime.Now.ToString("'Martes' dd 'de' MMMM 'del' yyyy")).FontSize(13).Bold().FontColor(Colors.Red.Medium);

                    col.Item().AlignCenter().Text($"Periodo: {_desde:dd/MM/yyyy} hasta {_hasta:dd/MM/yyyy}").FontSize(11);
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Categoría ").FontSize(12);
                        text.Span(_categoria).FontSize(13).Bold();
                    });
                });

                // Icono Derecha (Tarjeta)
                row.RelativeItem(2).AlignRight().Text("🟥").FontSize(35);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25); // 1. Nueva columna para Numeración (#)
                    columns.RelativeColumn(3);  // 2. Equipo
                    columns.RelativeColumn(4);  // 3. Nombre y Apellido
                    columns.RelativeColumn(2);  // 4. Carnet
                    columns.RelativeColumn(2);  // 5. Fecha
                    columns.RelativeColumn(3);  // 6. Sanción
                });

                table.Header(header =>
                {
                    // Estilo de celda para el encabezado
                    static IContainer CellStyle(IContainer container) =>
                        container.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten4).Padding(4).AlignCenter();

                    header.Cell().Element(CellStyle).Text("#").Bold(); // Encabezado numeración
                    header.Cell().Element(CellStyle).Text("Equipo").Bold().FontColor(Colors.Red.Medium);
                    header.Cell().Element(CellStyle).Text("Nombre y Apellido").Bold().FontColor(Colors.Red.Medium);
                    header.Cell().Element(CellStyle).Text("Carnet").Bold().FontColor(Colors.Red.Medium);
                    header.Cell().Element(CellStyle).Text("Fecha").Bold().FontColor(Colors.Red.Medium);
                    header.Cell().Element(CellStyle).Text("Sanción").Bold().FontColor(Colors.Red.Medium);
                });

                int contador = 1; // Iniciamos el contador de filas

                foreach (var item in _datos)
                {
                    static IContainer DataCellStyle(IContainer container) =>
                        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignMiddle();

                    // 1. Numeración
                    table.Cell().Element(DataCellStyle).AlignCenter().Text(contador.ToString()).FontSize(10);
                    
                    // 2. Equipo
                    table.Cell().Element(DataCellStyle).Text(t => t.Span(item.Equipo).Bold().FontSize(10));
                    
                    // 3. Nombre
                    table.Cell().Element(DataCellStyle).Text(item.NombreCompleto.ToUpper()).FontSize(10);
                    
                    // 4. Carnet
                    table.Cell().Element(DataCellStyle).AlignCenter().Text(item.Carnet).FontSize(10);
                    
                    // 5. Fecha
                    table.Cell().Element(DataCellStyle).AlignCenter().Text($"F {item.NroFecha}\n{item.FechaBoletin}").FontSize(9);
                    
                    // 6. Sanción
                    table.Cell().Element(DataCellStyle).Text(item.SancionTexto.ToUpper()).FontSize(10);

                    contador++; // Incrementamos para la siguiente fila
                }
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
                        t.Span("Sistema ClubId - Generado el ").FontSize(8).Italic();
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