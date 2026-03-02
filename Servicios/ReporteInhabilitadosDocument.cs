using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models;

namespace ClubId.Services
{
    public class ReporteInhabilitadosDocument : IDocument
    {
        private readonly List<SancionReporteDto> _datos;
        private readonly DateTime _desde;
        private readonly DateTime _hasta;
        private readonly string _categoria;
        private readonly string _rutaLogo = "G:/Proyectos Web/ClubId_MySQL/Image/escudoLiga.png";

        public ReporteInhabilitadosDocument(List<SancionReporteDto> datos, DateTime desde, DateTime hasta, string categoria)
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
                // Logo
                row.RelativeItem(2).Column(c =>
                {
                    if (System.IO.File.Exists(_rutaLogo))
                        c.Item().Width(70).Image(_rutaLogo);
                    else
                        c.Item().Text("LOGO").FontSize(10).Italic();
                });

                // Títulos centrales
                row.RelativeItem(6).Column(col =>
                {
                    col.Item().AlignCenter().Text("ASOCIACION DEL NORTE DE VETERANOS").FontSize(14).Bold().FontColor("#A386C5");
                    col.Item().AlignCenter().Text("Y SENIOR DE FUTBOL – SALTA").FontSize(12).Bold().FontColor("#A386C5");
                    
                    col.Item().PaddingTop(5).AlignCenter().Text("BOLETÍN DE JUGADORES INHABILITADOS").FontSize(16).Bold().FontColor(Colors.Red.Medium);
                    
                    col.Item().AlignCenter().Text(text => {
                        text.Span("Categoría: ").FontSize(11);
                        text.Span(_categoria).FontSize(12).Bold();
                    });
                    col.Item().AlignCenter().Text($"Periodo: {_desde:dd/MM/yyyy} al {_hasta:dd/MM/yyyy}").FontSize(10).Italic();
                });

                // Icono de advertencia
                row.RelativeItem(2).AlignRight().Text("⚠️").FontSize(35);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingTop(15).Column(col => 
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Equipo
                        columns.RelativeColumn(4); // Nombre
                        columns.RelativeColumn(2); // Carnet
                        columns.RelativeColumn(3); // Fecha/Boletín
                        columns.RelativeColumn(3); // Sanción
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("EQUIPO").Bold().FontSize(9);
                        header.Cell().Element(CellStyle).Text("APELLIDO Y NOMBRE").Bold().FontSize(9);
                        header.Cell().Element(CellStyle).Text("DNI").Bold().FontSize(9);
                        header.Cell().Element(CellStyle).Text("RONDA/FECHA.").Bold().FontSize(9);
                        header.Cell().Element(CellStyle).Text("ESTADO / SANCIÓN").Bold().FontSize(9);

                        static IContainer CellStyle(IContainer container) =>
                            container.DefaultTextStyle(x => x.FontColor(Colors.White)).Background(Colors.Red.Medium).Border(1).Padding(4).AlignCenter();
                    });

                    foreach (var item in _datos)
                    {
                        table.Cell().Element(DataCellStyle).Text(item.Equipo).Bold().FontSize(8);
                        table.Cell().Element(DataCellStyle).Text(item.NombreCompleto.ToUpper()).FontSize(8);
                        table.Cell().Element(DataCellStyle).AlignCenter().Text(item.Carnet).FontSize(8);
                        table.Cell().Element(DataCellStyle).AlignCenter().Text($"F {item.NroFecha}\n{item.FechaBoletin}").FontSize(8);
                        
                        // Resaltado de Sanción
                        table.Cell().Element(DataCellStyle).Background(Colors.Grey.Lighten4).Text(t => 
                        {
                            t.Span(item.SancionTexto.ToUpper()).Bold().FontSize(8).FontColor(Colors.Red.Medium);
                        });

                        static IContainer DataCellStyle(IContainer container) =>
                            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignMiddle();
                    }
                });

                // Mensaje Legal al final de la tabla
                col.Item().PaddingTop(20).Border(1).BorderColor(Colors.Red.Lighten3).Background(Colors.Red.Lighten5).Padding(10).Text(t => 
                {
                    t.Span("IMPORTANTE: ").Bold().FontColor(Colors.Red.Medium);
                    t.Span("Los jugadores listados en este reporte se encuentran inhabilitados para participar en cualquier encuentro oficial. La inclusión de un jugador inhabilitado en la planilla de juego resultará en la pérdida de puntos para el equipo infractor conforme al reglamento vigente.").FontSize(9);
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(t => {
                    t.Span("Documento oficial generado por ClubId el ").FontSize(8).Italic();
                    t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).Italic();
                });

                row.RelativeItem().AlignRight().Text(x => {
                    x.Span("Página ").FontSize(9);
                    x.CurrentPageNumber().FontSize(9).Bold();
                    x.Span(" de ").FontSize(9);
                    x.TotalPages().FontSize(9).Bold();
                });
            });
        }
    }
}