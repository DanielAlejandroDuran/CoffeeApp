using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;

namespace CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities
{
    /// <summary>
    /// Entidad que representa las imágenes asociadas a las variedades de café
    /// Mapea a la tabla 'imagenes_variedad' en la base de datos
    /// </summary>
    public class VarietyImage
    {
        /// <summary>
        /// ID único de la imagen (Primary Key)
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID de la variedad a la que pertenece esta imagen (Foreign Key)
        /// </summary>
        [Required]
        public int VarietyId { get; set; }

        /// <summary>
        /// URL o ruta donde se encuentra almacenada la imagen
        /// Puede ser una URL completa o una ruta relativa
        /// </summary>
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional de la imagen
        /// Útil para accesibilidad y SEO (alt text)
        /// </summary>
        public string? Description { get; set; }

        // ===== PROPIEDADES DE NAVEGACIÓN =====

        /// <summary>
        /// Variedad de café a la que pertenece esta imagen
        /// </summary>
        [ForeignKey("VarietyId")]
        public virtual Variety? Variety { get; set; }

        // ===== PROPIEDADES CALCULADAS =====

        /// <summary>
        /// Obtiene el nombre del archivo desde la URL
        /// </summary>
        public string NombreArchivo => 
            Path.GetFileName(ImageUrl) ?? "imagen_sin_nombre";

        /// <summary>
        /// Obtiene la extensión del archivo de imagen
        /// </summary>
        public string Extension => 
            Path.GetExtension(ImageUrl)?.ToLowerInvariant() ?? "";

        /// <summary>
        /// Verifica si la URL apunta a una imagen válida basándose en la extensión
        /// </summary>
        public bool EsImagenValida =>
            !string.IsNullOrEmpty(Extension) &&
            (Extension == ".jpg" || Extension == ".jpeg" || Extension == ".png" || 
             Extension == ".gif" || Extension == ".webp" || Extension == ".svg");

        /// <summary>
        /// Verifica si la imagen es una URL externa (http/https) o local
        /// </summary>
        public bool EsUrlExterna => 
            ImageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            ImageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        // ===== MÉTODOS ÚTILES =====

        /// <summary>
        /// Obtiene el texto alternativo para la imagen, usando la descripción si está disponible
        /// </summary>
        /// <returns>Texto alternativo para la imagen</returns>
        public string GetAltText()
        {
            if (!string.IsNullOrEmpty(Description))
                return Description;
            
            return Variety != null 
                ? $"Imagen de la variedad {Variety.CommonName ?? "Sin nombre"}"
                : "Imagen de variedad de café";
        }

        /// <summary>
        /// Valida que la URL de la imagen sea válida
        /// </summary>
        /// <returns>True si la URL es válida, false en caso contrario</returns>
        public bool ValidarUrl()
        {
            if (string.IsNullOrWhiteSpace(ImageUrl))
                return false;

            // Si es URL externa, verificar que sea válida
            if (EsUrlExterna)
            {
                return Uri.TryCreate(ImageUrl, UriKind.Absolute, out var uri) &&
                       (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            }

            // Si es ruta local, verificar que no tenga caracteres inválidos
            return !string.IsNullOrEmpty(ImageUrl) && 
                   ImageUrl.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }
    }
}