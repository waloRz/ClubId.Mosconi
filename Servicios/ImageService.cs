using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp; // Asegúrate que este paquete esté instalado

namespace ClubId.Services
{
    public interface IImageService
    {
        Task<string> SubirFotoPerfil(IFormFile archivo, string carpetaDestino, bool recortar = true);
    }

    public class ImageService : IImageService
    {
        public async Task<string> SubirFotoPerfil(IFormFile archivo, string carpetaDestino, bool recortar = true)
        {
            // 1. Usamos SixLabors.ImageSharp.Image para evitar conflictos de nombre
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(archivo.OpenReadStream());

            var modoRedimensionado = recortar ? ResizeMode.Crop : ResizeMode.Pad;

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(400, 400),
                Mode = modoRedimensionado
            }));

            var nombreArchivo = $"{Guid.NewGuid()}.webp";
            
            // Aseguramos que la carpeta exista antes de guardar
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            // 2. Si WebPEncoder sigue fallando, prueba instanciarlo así:
            await image.SaveAsWebpAsync(rutaCompleta, new WebpEncoder 
            { 
                Quality = 80 
            });

            return nombreArchivo;
        }
    }
}