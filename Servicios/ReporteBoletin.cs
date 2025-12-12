using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models.ViewModels;
using QuestPDF.Companion;
using System.ComponentModel.DataAnnotations;

namespace ClubId.Services
{

// 1. Definimos el Modelo de Datos (ViewModel)
public class BoletinSancionesModel
{
    public string FechaTexto { get; set; } // "Martes 18 de Noviembre del 2025"
    public string Categoria { get; set; }  // "VETERANOS"
    
    public string NroFecha { get; set; }   // "FECHA 4"
    public List<SancionItem> Sanciones { get; set; } = new List<SancionItem>();
}

public class SancionItem
{
    public string Club { get; set; }
    public string Jugador { get; set; } //nombre
    public string Carnet { get; set; }
    public string Sancion { get; set; }
}

    // 2. Definimos el Documento
    public class ReporteBoletin : IDocument
    {
        public BoletinSancionesModel Model { get; }

        // Colores extraídos de la imagen
        private static readonly string ColorVioleta = "#730D73"; // Tono lila/violeta suave
        private static readonly string ColorRojo = "#D32F2F";
        private static readonly string ColorNaranja = "#F57C00"; // Naranja para las líneas
        private static readonly string ColorAzulClub = "#1976D2";

        public ReporteBoletin(BoletinSancionesModel model)
        {
            Model = model;
        }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
   

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // 1. Logo Izquierda
               row.ConstantItem(110).Image("G:/Proyectos Web/ClubId_MySQL/Image/escudoLiga.png"); // REEMPLAZAR CON TU LOGO: .Image("logo.png")

                // 2. Texto Central
                row.RelativeItem().PaddingLeft(20).PaddingRight(5).Column(col =>
                {
                    col.Item().AlignCenter().Text("ASOCIACION DEL NORTE DE VETERANOS")
                       .FontSize(14).Bold().FontColor(ColorVioleta);

                    col.Item().AlignCenter().Text("Y SENIOR DE FUTBOL – SALTA")
                       .FontSize(14).Bold().FontColor(ColorVioleta);

                    col.Item().AlignCenter().Text("REUNION DEL TRIBUNAL DE PENA")
                       .FontSize(13).Bold().FontColor(ColorVioleta);

                    col.Item().PaddingVertical(5).AlignCenter().Text(Model.FechaTexto.ToUpper())
                       .FontSize(14).Bold().FontColor(ColorRojo);

                    col.Item().AlignCenter().Text("En la reunión del tribunal de pena se resolvió lo siguiente");

                    col.Item().PaddingTop(5).AlignCenter().Text(txt =>
                    {
                        txt.Span("Categoría ").FontSize(13);
                        txt.Span(Model.Categoria).FontSize(14).Bold();
                        txt.Span($" “{Model.NroFecha}”").FontSize(16).Bold().FontColor(ColorRojo);
                    });
                });

                // 3. Imagen Tarjeta Roja Derecha (Opcional, basado en la imagen)
               row.ConstantItem(110).AlignRight().Image("G:/Proyectos Web/ClubId_MySQL/Image/tarjetaRoja.png"); // REEMPLAZAR: .Image("tarjeta.png")
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingTop(20).Table(table =>
            {
                // Definimos 2 columnas grandes para la hoja (como en la imagen)
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(15); // Espacio separador entre las dos columnas de datos
                    columns.RelativeColumn();
                });

                // Iteramos los datos. 
                // Usamos un índice para saber si dibujar en la columna izq o der, 
                // pero QuestPDF fluye automáticamente si llenamos celdas.
                // Para forzar el "Gap" (hueco) central, simplemente añadimos una celda vacía en medio.

                for (int i = 0; i < Model.Sanciones.Count; i += 2)
                {
                    // Item Izquierdo
                    table.Cell().Element(c => ComposeSancionCard(c, Model.Sanciones[i]));

                    // Espacio Central (Separador visual naranja vertical si quisieras, aquí es vacío)
                    table.Cell();

                    // Item Derecho (Verificar si existe para no romper el loop si es impar)
                    if (i + 1 < Model.Sanciones.Count)
                    {
                        table.Cell().Element(c => ComposeSancionCard(c, Model.Sanciones[i + 1]));
                    }
                    else
                    {
                        table.Cell(); // Celda vacía si es impar
                    }

                    // Fila separadora pequeña vertical entre bloques
                    table.Cell().ColumnSpan(3).Height(11);
                }
            });
        }

        // Este método dibuja el recuadro naranja individual de cada jugador
        void ComposeSancionCard(IContainer container, SancionItem item)
        {
            // La "Ficha" es una tabla interna
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55); // Ancho para las etiquetas (Club, Jugador...)
                    columns.RelativeColumn();   // El resto para el valor
                });

                // Definimos un estilo común para bordes naranjas
                static IContainer CellStyle(IContainer c) => c
                    .BorderBottom(1).BorderColor(ColorNaranja)
                    .PaddingLeft(2).Height(20);

                static IContainer LabelStyle(IContainer c) => CellStyle(c)
                    .BorderRight(2).BorderColor(ColorNaranja); // Línea vertical naranja

                // --- FILA 1: CLUB ---
                table.Cell().Element(LabelStyle).Text("Club ").Bold().FontSize(13);
                table.Cell().Element(CellStyle).Text(item.Club).Bold().FontColor(ColorAzulClub).AlignCenter().FontSize(16);

                // --- FILA 2: JUGADOR ---
                table.Cell().Element(LabelStyle).Text("Jugador").Bold().FontSize(14);
                table.Cell().PaddingLeft(3).Element(CellStyle).Text(item.Jugador);

                // --- FILA 3: CARNET ---
                table.Cell().Element(LabelStyle).Text("Carnet ").Bold().FontSize(14);
                table.Cell().PaddingLeft(3).Element(CellStyle).Text(item.Carnet);

                // --- FILA 4: SANCION (Sin borde inferior en la última fila visualmente, o sí, según gusto)
                // En la imagen parece que la última línea naranja también existe.
             
               table.Cell().BorderRight(2).BorderColor(ColorNaranja).Padding(3).Text("Sanción ").Bold();
                //table.Cell().Element(CellStyle).Text(item.Sancion);
                table.Cell().PaddingLeft(1).Padding(3).Text(item.Sancion);
            });
        }
   
      public void Compose(IDocumentContainer container)
        {
        //   var document = Document.Create(container =>             // COMENTAR ACA Y ABAJO PARA GENERAR EL PDF
        //     {                       // COMENTAR ACA PARA GENERAR EL PDF
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(10);
                    page.DefaultTextStyle(x => x.FontSize(13).FontFamily(Fonts.TimesNewRoman));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        //     });            // COMENTAR ACA Y ABAJO PARA GENERAR EL PDF
        //    document.ShowInCompanion();          // COMENTAR ACA PARA GENERAR EL PDF
       }
    }

}