using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClubId.Models.ViewModels;

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
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Portrait());
                page.Margin(10); 

                // --- NUEVA LÓGICA DE COLOR DINÁMICO ---
                // Si el modelo trae un color (ej: "#FF5733"), lo usamos. 
                // Si no, usamos Rojo como color de respaldo.
                string colorHex = !string.IsNullOrEmpty(Model.Color) ? Model.Color : "#000000";
                
                page.Content()
                    .Height(170)
                    .Border(2)
                    .BorderColor(Colors.Black)
                    .Padding(5)
                    .Background(Colors.White)
                    .Row(row =>
                    {
                        // Lado Izquierdo (Frente)
                        row.RelativeItem()
                            .Border(3)
                            .BorderColor(colorHex) // <-- Usamos el hexadecimal directamente
                            .Padding(10)
                            .Column(frontColumn =>
                            {
                                frontColumn.Item()
                                    .Background(Colors.White)
                                    .BorderBottom(2)
                                    .BorderColor(colorHex) // <-- Borde dinámico
                                    .PaddingBottom(5)
                                    .Text((Model.Nombre ?? "").ToUpper() + " " + (Model.Apellido ?? "").ToUpper())
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

                        row.ConstantItem(10); // Separador

                        // Lado Derecho (Dorso)
                        row.RelativeItem()
                            .BorderColor(colorHex) // <-- Borde dinámico
                            .Border(3)
                            .Padding(5)
                            .Column(backColumn =>
                            {
                                backColumn.Item()
                                    .BorderBottom(2)
                                    .BorderColor(colorHex) // <-- Línea dinámica
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
                                                string categoria = Model.NombreCat?.ToUpper() ?? "S/C";
                                                string inicialCat = categoria.Length > 0 ? categoria.Substring(0, 1) : "C";

                                                infoColumn.Item().Text($"CARNET N°:  {inicialCat}- {Model.JugadorId}").Bold();
                                                infoColumn.Item().PaddingTop(7).Text($"CATEGORIA:");
                                                infoColumn.Item().PaddingTop(3).Text(categoria).Bold()
                                                    .FontSize(12).AlignCenter();
                                                
                                                infoColumn.Item().PaddingTop(8).Text("EQUIPO");
                                                infoColumn.Item().PaddingTop(3).Text(Model.NombreEquipo?.ToUpper() ?? "").Bold()
                                                    .FontSize(12).AlignCenter();
                                            });

                                        // Foto de perfil
                                        string imagePath = Path.Combine(_webRootPath, "fotosPerfiles", Model.Foto ?? "");
                                        if (!string.IsNullOrEmpty(Model.Foto) && File.Exists(imagePath))
                                        {
                                            dataRow.RelativeItem().Width(130).Height(105).PaddingLeft(10).Image(imagePath).FitUnproportionally(); 
                                        }
                                        else
                                        {
                                            dataRow.RelativeItem().Width(130).Height(100).PaddingVertical(35).PaddingLeft(15).Text("Foto no disponible").FontSize(8).Italic();
                                        }
                                    });
                            });
                    });
            });
        }
    }
}