using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models.ViewModels; // Asegúrate de que este sea el namespace de tu ViewModel

namespace ClubId.Services
{
    public class ReporteExpulsadosDocument : IDocument
    {
        private readonly ReporteExpulsadosModel _model;
        private readonly string _webRootPath;

        public ReporteExpulsadosDocument(ReporteExpulsadosModel model, string webRootPath)
        {
            _model = model;
            _webRootPath = webRootPath;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            // Ruta física para el escudo de la liga
            var rutaAbsolutaLogo = Path.Combine(_webRootPath, "imgFijas", "escudoLiga.png");

            container.Row(row =>
            {
                // 1. Logo Izquierda
                row.RelativeItem(2).Column(c =>
                {
                    if (File.Exists(rutaAbsolutaLogo))
                        c.Item().Width(70).Image(rutaAbsolutaLogo);
                    else
                        c.Item().PaddingTop(10).Text("LOGO").FontSize(8).Italic().FontColor(Colors.Grey.Medium);
                });

                // 2. Títulos Centrales
                row.RelativeItem(6).Column(col =>
                {
                    col.Item().AlignCenter().Text("ASOCIACION DEL NORTE DE VETERANOS").FontSize(14).Bold().FontColor("#A386C5");
                    col.Item().AlignCenter().Text("Y SENIOR DE FUTBOL – SALTA").FontSize(12).Bold().FontColor("#A386C5");
                    
                    col.Item().PaddingTop(5).AlignCenter().Text("NÓMINA DE JUGADORES EXPULSADOS").FontSize(16).Bold().FontColor(Colors.Red.Medium);
                    
                    col.Item().AlignCenter().Text(text => {
                        text.Span("Categoría: ").FontSize(11);
                        // Usamos la categoría del primer jugador o "GENERAL" si no hay ninguno
                        var catNombre = _model.Jugadores.FirstOrDefault()?.Categoria ?? "TODAS";
                        text.Span(catNombre.ToUpper()).FontSize(12).Bold();
                    });
                });

                // 3. Icono Derecha
                row.RelativeItem(2).AlignRight().Text("🚫").FontSize(35);
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
                columns.ConstantColumn(25); // Nueva columna para el N°
                columns.RelativeColumn(3);  // Equipo
                columns.RelativeColumn(4);  // Nombre
                columns.RelativeColumn(2);  // DNI
                columns.RelativeColumn(3);  // Ronda/Fecha
                columns.RelativeColumn(3);  // Sancion
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("N°").Bold().FontSize(9); // Encabezado N°
                header.Cell().Element(CellStyle).Text("EQUIPO").Bold().FontSize(9);
                header.Cell().Element(CellStyle).Text("APELLIDO Y NOMBRE").Bold().FontSize(9);
                header.Cell().Element(CellStyle).Text("DNI").Bold().FontSize(9);
                header.Cell().Element(CellStyle).Text("RONDA / FECHA").Bold().FontSize(9);
                header.Cell().Element(CellStyle).Text("SANCION").Bold().FontSize(9);

                static IContainer CellStyle(IContainer container) =>
                    container.DefaultTextStyle(x => x.FontColor(Colors.White)).Background(Colors.Red.Medium).Border(1).Padding(4).AlignCenter();
            });

            // VARIABLE PARA EL CONTADOR
            int nro = 1;

            foreach (var item in _model.Jugadores)
            {
                // Celda de numeración
                table.Cell().Element(DataCellStyle).AlignCenter().Text(nro.ToString()).FontSize(8);
                
                table.Cell().Element(DataCellStyle).Text(item.Equipo).Bold().FontSize(8);
                table.Cell().Element(DataCellStyle).Text(item.NombreCompleto).FontSize(8);
                table.Cell().Element(DataCellStyle).AlignCenter().Text(item.Dni).FontSize(8);
                
                table.Cell().Element(DataCellStyle).AlignCenter().Column(c => {
                    c.Item().Text(item.RondaFecha).Bold().FontSize(8);
                    c.Item().Text(item.FechaBoletin).FontSize(7).Italic();
                });
                
                table.Cell().Element(DataCellStyle).Background(Colors.Grey.Lighten4).AlignCenter()
                    .Text(item.Sancion).Bold().FontSize(8).FontColor(Colors.Red.Medium);

                nro++; // INCREMENTAMOS EL NÚMERO PARA LA SIGUIENTE FILA

                static IContainer DataCellStyle(IContainer container) =>
                    container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignMiddle();
            }
        });

                // Cuadro de Aviso Legal al final
                col.Item().PaddingTop(20).Border(1).BorderColor(Colors.Red.Lighten3).Background(Colors.Red.Lighten5).Padding(10).Text(t => 
                {
                    t.Span("IMPORTANTE: ").Bold().FontColor(Colors.Red.Medium);
                    t.Span("Los jugadores listados han sido EXPULSADOS de la liga. Esta condición es permanente a menos que el Tribunal de Penas indique lo contrario. No están habilitados para participar en ninguna categoría.").FontSize(9);
                });

            });
        }

        void ComposeFooter(IContainer container)
        {
            container.PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(t => {
                    t.Span("Documento oficial generado por ClubId - Salta | ").FontSize(8).Italic();
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