using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using ClubId.Services;

namespace ClubId.Services
{
    public interface IImageService
    {
        Task<string> SubirFotoPerfil(IFormFile archivo, string folderPath);
    }

    public class ImageService : IImageService
    {
        public async Task<string> SubirFotoPerfil(IFormFile archivo, string folderPath)
        {
            // 1. Si no hay archivo, devolvemos una imagen por defecto
            if (archivo == null || archivo.Length == 0) return "default-user.webp";

            // 2. Generar nombre único y asegurar que la carpeta exista
            var nombreArchivo = $"{Guid.NewGuid()}.webp";
            var rutaCompleta = Path.Combine(folderPath, nombreArchivo);

            if (!Directory.Exists(folderPath)) 
                Directory.CreateDirectory(folderPath);

            try 
            {
                using (var image = await Image.LoadAsync(archivo.OpenReadStream()))
                {
                    // 3. Recorte inteligente (Smart Crop)
                    // Buscamos que la foto sea 400x400 sin estirarse
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(400, 400),
                        Mode = ResizeMode.Crop // Corta los sobrantes para que sea cuadrada
                    }));

                    // 4. Guardar como WebP
                    await image.SaveAsWebpAsync(rutaCompleta, new WebpEncoder
                    {
                        Quality = 80 // Excelente balance entre calidad y peso (~30KB)
                    });
                }
                return nombreArchivo;
            }
            catch (Exception)
            {
                // Si el archivo estaba corrupto o no era imagen, devolvemos el default
                return "default-user.webp";
            }
        }
    }
}