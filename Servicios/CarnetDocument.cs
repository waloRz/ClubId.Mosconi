using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models.ViewModels;
using QuestPDF.Companion;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClubId.Services
{
    public class CarnetDocument : IDocument
    {
        public JugadorCarnetViewModel Model { get; }
        private readonly string _webRootPath;

        public CarnetDocument(JugadorCarnetViewModel model, string webRootPath)
        {
            Model = model;
            _webRootPath = webRootPath;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            //var document = Document.Create(container =>  // COMENTAR ACA PARA GENERAR EL PDF
            // {                                                                                                         // COMENTAR ACA
            container.Page(page =>
                {
                    page.Size(PageSizes.A4.Portrait());
                    page.Margin(10); 

                    page.Content()
                    .Height(170)
                        .Border(2)
                        .BorderColor(Colors.Black)
                        .Padding(5)
                        .Background(Colors.White)
                        .Row(row =>
                        {
                            // --- NUEVO BLOQUE DE LÓGICA DE COLOR ---
                            // Definimos la categoría en mayúsculas para evitar errores de tipeo
                            string categoria = Model.NombreCat?.ToUpper() ?? "";
                            string colorMarco;

                            if (categoria.Contains("SUPER-60"))
                            {
                                colorMarco = Colors.Blue.Medium;
                            }
                            else if (categoria.Contains("SUPER V 42"))
                            {
                                // Usamos Darken1 para que resalte mejor al imprimir sobre blanco
                                colorMarco = Colors.Yellow.Darken1; 
                            }
                            else 
                            {
                                // Por defecto (Veteranos u otra categoría)
                                colorMarco = Colors.Red.Medium;
                            }
                            // ----------------------------------------

                            // Left side of the carnet (Front)
                            row.RelativeItem()
                            .Border(3)
                            .BorderColor(colorMarco) // Aplicamos el color aquí
                            .Padding(10)
                            .Column(frontColumn =>
                                {
                                    frontColumn.Item()
                                    .Background(Colors.White)
                                        .BorderBottom(2)
                                      .BorderColor(colorMarco) // Aplicamos el color aquí
                                        .PaddingBottom(5)
                                        .Text(Model.Nombre.ToUpper() + " " + Model.Apellido.ToUpper())
                                        .Bold()
                                        .FontSize(14)
                                        .AlignCenter();

                                    frontColumn.Item()
                                        .PaddingTop(10)
                                        .Row(dataRow =>
                                        {
                                            dataRow.RelativeItem(2)
                                                .Column(infoColumn =>
                                                {
                                                    infoColumn.Item().PaddingTop(2).Text($"D.N.I. N°:   {Model.Dni}");
                                                    infoColumn.Item().PaddingTop(2).Text($"FECHA NACIMIENTO:  {Model.FechaNac.ToShortDateString()}");
                                                    infoColumn.Item().PaddingTop(2).Text($"FECHA INSCRIPCIÓN:  {Model.FechaRecibo.ToShortDateString()}");
                                                });

                                            dataRow.ConstantItem(10); 
                                        });

                                    frontColumn.Item()
                                        .Background(Colors.Grey.Lighten3)
                                        .PaddingTop(40)
                                        .AlignMiddle()
                                        .Row(footerRow =>
                                        {
                                            footerRow.RelativeItem().Text("JUGADOR").Bold().AlignCenter();
                                            footerRow.RelativeItem().AlignCenter().Text("PRESIDENTE").Bold();
                                        });
                                });

                            row.ConstantItem(10); // Separador entre los dos carnets

                            // Lado derecho del carnet.
                            row.RelativeItem()
                            .BorderColor(colorMarco) // Aplicamos el mismo color al lado derecho
                            .Border(3)
                                .Padding(5)
                                .Column(backColumn =>
                                {
                                    backColumn.Item()
                                     .BorderBottom(2)
                                      .BorderColor(colorMarco) // Y a la línea separadora
                                        .Text("ASOCIACION DEL NORTE DE VETERANOS SUPER VETERANOS Y SENIOR DE FUTBOL-SALTA")
                                        .Bold()
                                        .FontSize(11)
                                        .AlignCenter();

                                    backColumn.Item()
                                        .PaddingTop(16)
                                        .Row(dataRow =>
                                        {
                                            dataRow.RelativeItem(1)
                                                .Column(infoColumn =>
                                                {
                                                    // Extraemos la primera letra de forma segura. Si por algún motivo la categoría está vacía, ponemos una "C" por defecto.
                                                    string inicialCat = categoria.Length > 0 ? categoria.Substring(0, 1) : "C";

                                                    infoColumn.Item().Text($"CARNET N°:  {inicialCat}- {Model.JugadorId}").Bold();

                                                    infoColumn.Item().PaddingTop(7).Text($"CATEGORIA:");
                                                 // Usamos la variable 'categoria' que ya procesamos al principio del Compose
                                                    infoColumn.Item().PaddingTop(3).Text(categoria).Bold()
                                                        .FontSize(12).AlignCenter();
                                
                                                    infoColumn.Item().PaddingTop(8).Text("EQUIPO");
                                                    infoColumn.Item().PaddingTop(3).Text(Model.NombreEquipo.ToUpper()).Bold()
                                                        .FontSize(12).AlignCenter();
                                                });

                                            // Manejo de la imagen de perfil
                                            string imagePath = Path.Combine(_webRootPath, "fotosPerfiles", Model.Foto ?? "");
                                            bool imageExists = !string.IsNullOrEmpty(Model.Foto) && File.Exists(imagePath);

                                            if (imageExists)
                                            {
                                                dataRow.RelativeItem()
                                                    .Width(130)
                                                    .Height(105)
                                                    .PaddingLeft(10)
                                                    .Image(imagePath)
                                                    .FitUnproportionally(); 
                                            }
                                            else
                                            {
                                                dataRow.RelativeItem()
                                                    .Width(130)
                                                    .Height(100)
                                                    .PaddingVertical(35)
                                                    .PaddingLeft(15)
                                                    .Text("Foto no disponible")
                                                    .FontSize(8)
                                                    .Italic();
                                            }
                                        });
                                });
                        });
                });
            //}); // COMENTAR ACA PARA GENERAR EL PDF

            //document.ShowInCompanion(); // COMENTAR ACA PARA GENERAR EL PDF
        }
    }
}