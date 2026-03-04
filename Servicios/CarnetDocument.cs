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
            // {                                                                               // COMENTAR ACA
            container.Page(page =>
                {
                    // Ajusta el tamaño de la página para que se asemeje a un carnet, por ejemplo, A5 en horizontal
                    page.Size(PageSizes.A4.Portrait());
                    page.Margin(10); // Minimal margin

                    page.Content()
                    .Height(170)
                        .Border(2)
                        .BorderColor(Colors.Black)
                        .Padding(5)
                        .Background(Colors.White)
                        .Row(row =>
                        {
                            // Left side of the carnet (Front)
                            var Categoria = Model.NombreCat;
                            var color = "";
                            if (Categoria != "SUPER-60")
                            {
                                color = Colors.Red.Medium;
                            }
                            else
                            {
                                color = Colors.Blue.Medium;
                            }
                            row.RelativeItem()
                            .Border(3)
                            .BorderColor(color)
                            .Padding(10)
                            .Column(frontColumn =>
                                {
                                    frontColumn.Item()
                                    .Background(Colors.White)
                                        .BorderBottom(2)
                                      .BorderColor(color)
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

                                            dataRow.ConstantItem(10); // Espacio entre texto e imagen

                                            // Association Logo
                                            // dataRow.RelativeItem(1)
                                            //     .Height(50)
                                            //     .AlignRight()
                                            //     .AlignMiddle()
                                            //     .Image(_logoAsociacion)
                                            //     .FitUnproportionally();
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

                            if (Categoria != "SUPER-60")
                            {
                                color = Colors.Red.Medium;
                            }
                            else
                            {
                                color = Colors.Blue.Medium;
                            }
                            row.RelativeItem()
                            .BorderColor(color)
                            .Border(3)
                                // .BorderBottom(2)
                                .Padding(5)
                                .Column(backColumn =>
                                {
                                    backColumn.Item()
                                     .BorderBottom(2)

                                      .BorderColor(color)
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
                                                    infoColumn.Item().Text($"CARNET N°:  {Model.NombreCat.First()}- {Model.JugadorId}").Bold();

                                                    infoColumn.Item().PaddingTop(7).Text($"CATEGORIA:");
                                                    infoColumn.Item().PaddingTop(3).Text(Model.NombreCat.ToUpper()).Bold()
                                                        .FontSize(12).AlignCenter();
                                                    infoColumn.Item().PaddingTop(8).Text("EQUIPO");
                                                    infoColumn.Item().PaddingTop(3).Text(Model.NombreEquipo.ToUpper()).Bold()
                                                        .FontSize(12).AlignCenter();
                                                });

                                            //     dataRow.ConstantItem(10);

                                            // 1. Construimos la ruta dinámica. 
                                            // Model.Foto ahora solo tiene el nombre (ej: "archivo.webp")
                                            string imagePath = Path.Combine(_webRootPath, "fotosPerfiles", Model.Foto ?? "");
                                            bool imageExists = !string.IsNullOrEmpty(Model.Foto) && File.Exists(imagePath);

                                            if (imageExists)
                                            {
                                                dataRow.RelativeItem()
                                                    .Width(130)
                                                    .Height(105)
                                                    .PaddingLeft(10)
                                                    .Image(imagePath)
                                                    .FitUnproportionally(); // O FitWidth() si quieres mantener el ratio
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