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

        public CarnetDocument(JugadorCarnetViewModel model)
        {
            Model = model;
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
                                var Categoria = Model.nombreCat;
                                var color = "";
                                if (Categoria != "Super-45")
                                {
                                    color = Colors.Red.Medium;
                                } else
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
                              //  var Categoria = Model.nombreCat; 
                                //var color = "";
                                         if (Categoria !="Super-45" )
                                         {
                                     color = Colors.Red.Medium;
                                         }else
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
                                                        infoColumn.Item().Text($"CARNET N°: {Model.JugadorId}").Bold();

                                                        infoColumn.Item().PaddingTop(7).Text($"CATEGORIA:");
                                                            infoColumn.Item().PaddingTop(3).Text(Model.nombreCat.ToUpper()).Bold()
                                                            .FontSize(12).AlignCenter();
                                                            infoColumn.Item().PaddingTop(8).Text("EQUIPO");
                                                            infoColumn.Item().PaddingTop(3).Text(Model.NombreEquipo.ToUpper()).Bold()
                                                            .FontSize(12).AlignCenter();                                                      
                                                    });

                                                //     dataRow.ConstantItem(10);

                                                string imagePath = "G:/Proyectos Web/ClubId_MySQL/wwwroot/" + Model.Foto;
                                                bool imageExists = File.Exists(imagePath);

                                                if (imageExists)
                                                {
                                                    // Player's photo
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
                                                    .ShowIf(!imageExists)
                                                    .Text("Imagen no disponible");
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