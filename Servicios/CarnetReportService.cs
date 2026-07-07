using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace ClubId.Services
{
    // DTO específico para transportar los datos necesarios al reporte
    public class JugadorCarnetDto
    {
        public int IdJugador { get; set; }
        public string NroCarnet { get; set; } = null!;
        public string ApellidoYNombre { get; set; }= null!;
        public string Dni { get; set; }= null!;
        public string FechaNacimiento { get; set; }= null!;
        
        // La foto puede venir como un array de bytes desde la Base de Datos (Blob)
        // Si no tiene foto, el servicio manejará una imagen por defecto (silueta)
        public byte[] FotoBytes { get; set; } = null!;
        public string? Foto { get; set; }
    }

    public class CarnetReportService
    {
        private readonly string _placeholderImagePath;

        // Inyectamos la ruta base para buscar una imagen por defecto si el jugador no tiene foto
        public CarnetReportService(string webRootPath)
        {
            // Asegúrate de tener una silueta en wwwroot/images/default-avatar.png o similar
            _placeholderImagePath = Path.Combine(webRootPath, "images", "default-avatar.png");
        }

        public byte[] GenerarPdfCarnets(List<JugadorCarnetDto> jugadores)
        {
            // Si la lista viene vacía, generamos un documento en blanco seguro
            if (jugadores == null || !jugadores.Any())
                jugadores = new List<JugadorCarnetDto>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configuración de la página A4 (210mm x 297mm)
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre); // 10mm de margen periférico
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Content().PaddingVertical(5).Table(table =>
                    {
                        // Definimos 2 columnas iguales (ocupan el 50% del ancho disponible cada una)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Iteramos sobre la lista de jugadores
                        foreach (var jugador in jugadores)
                        {
                            table.Cell().Padding(6).Element(container => EscupirCarnet(container, jugador));
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        // Método encargado de dibujar la estructura visual de UN solo carnet
        private void EscupirCarnet(IContainer container, JugadorCarnetDto jugador)
        {
            // Dimensiones matemáticas para que entren exactamente 4 por hoja A4 contando márgenes:
            // Alto máximo recomendable por celda: ~132mm
            container
                .Height(130, Unit.Millimetre) 
                .Border(1)
                .BorderColor(Colors.Grey.Darken1)
                .Background(Colors.Grey.Lighten5)
                .CornerRadius(4)
                .Column(col =>
                {
                    // ENCABEZADO DEL CARNET
                    col.Item().Background(Colors.Blue.Darken3).Padding(6).Row(row =>
                    {
                        row.RelativeItem().Text("CARNET DE IDENTIFICACIÓN")
                            .Bold()
                            .FontSize(11)
                            .FontColor(Colors.White)
                            .AlignCenter();
                    });

                    // CUERPO DEL CARNET (Foto a la izquierda, datos a la derecha)
                    col.Item().Padding(8).Row(row =>
                    {
                        // Columna de la Foto (Ancho fijo de 65mm para mantener proporción tipo credencial)
                        row.ConstantItem(65, Unit.Millimetre).Column(colFoto =>
                        {
                            colFoto.Item()
                                .Width(60, Unit.Millimetre)
                                .Height(75, Unit.Millimetre)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten1)
                                .Background(Colors.Grey.Lighten3)
                                .Element(imgContainer => 
                                {
                                    // Validamos si el jugador tiene foto en bytes, sino cargamos el placeholder
                                    if (jugador.FotoBytes != null && jugador.FotoBytes.Length > 0)
                                    {
                                        imgContainer.Image(jugador.FotoBytes);
                                    }
                                    else if (File.Exists(_placeholderImagePath))
                                    {
                                        imgContainer.Image(_placeholderImagePath);
                                    }
                                    else
                                    {
                                        // Si falla todo, dejamos el recuadro gris limpio
                                        imgContainer.Background(Colors.Grey.Lighten2);
                                    }
                                });

                            // Número de Carnet / ID destacado abajo de la foto
                            colFoto.Item().PaddingTop(8).AlignCenter().Text(t =>
                            {
                                t.Span("Nº CARNET: ").Bold().FontSize(10);
                                t.Span(jugador.NroCarnet ?? jugador.IdJugador.ToString()).Bold().FontSize(12).FontColor(Colors.Red.Medium);
                            });
                        });

                        // Espaciador entre Foto y Datos
                        row.ConstantItem(10);

                        // Columna de Datos Personales
                        row.RelativeItem().Column(colDatos =>
                        {
                            colDatos.Item().PaddingTop(5);
                            
                            colDatos.Item().Text("APELLIDO Y NOMBRE:").Bold().FontSize(8).FontColor(Colors.Grey.Darken2);
                            colDatos.Item().PaddingBottom(6).Text((jugador.ApellidoYNombre ?? "S/D").ToUpper()).Bold().FontSize(11);

                            colDatos.Item().Text("D.N.I.:").Bold().FontSize(8).FontColor(Colors.Grey.Darken2);
                            colDatos.Item().PaddingBottom(6).Text(jugador.Dni ?? "S/D").FontSize(11);

                            colDatos.Item().Text("FECHA DE NACIMIENTO:").Bold().FontSize(8).FontColor(Colors.Grey.Darken2);
                            colDatos.Item().PaddingBottom(6).Text(jugador.FechaNacimiento ?? "S/D").FontSize(11);
                        });
                    });

                    // PIE DEL CARNET (Espacio para firmas, sellos o fechado de vigencia)
                    col.Item().BorderTop(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(6).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Válido Torneo Actual").FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                        row.RelativeItem().AlignRight().Text("Firma Autorizada ________________").FontSize(8).FontColor(Colors.Grey.Darken1);
                    });
                });
        }
    }
}