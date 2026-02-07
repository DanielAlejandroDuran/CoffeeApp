using System.ComponentModel.DataAnnotations;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
    /// <summary>
    /// DTO para crear una nueva variedad de café
    /// </summary>
    public class CreateVarietyDto
    {
        /// <summary>
        /// Nombre común de la variedad de café
        /// </summary>
        [Required(ErrorMessage = "El nombre común es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre común no puede exceder 100 caracteres")]
        public string NombreComun { get; set; } = string.Empty;

        /// <summary>
        /// Nombre científico de la variedad
        /// </summary>
        [StringLength(150, ErrorMessage = "El nombre científico no puede exceder 150 caracteres")]
        public string? NombreCientifico { get; set; }

        /// <summary>
        /// Descripción detallada de la variedad
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Porte o altura de la planta
        /// </summary>
        [Required(ErrorMessage = "El porte es obligatorio")]
        public PlantHeight Porte { get; set; }

        /// <summary>
        /// Tamaño del grano de café
        /// </summary>
        [Required(ErrorMessage = "El tamaño del grano es obligatorio")]
        public GrainSize TamanoGrano { get; set; }

        /// <summary>
        /// Altitud mínima recomendada (metros sobre el nivel del mar)
        /// </summary>
        [Range(0, 5000, ErrorMessage = "La altitud mínima debe estar entre 0 y 5000 metros")]
        public int? AltitudMin { get; set; }

        /// <summary>
        /// Altitud máxima recomendada (metros sobre el nivel del mar)
        /// </summary>
        [Range(0, 5000, ErrorMessage = "La altitud máxima debe estar entre 0 y 5000 metros")]
        public int? AltitudMax { get; set; }

        /// <summary>
        /// Potencial de rendimiento de la variedad
        /// </summary>
        public YieldPotential? Rendimiento { get; set; }

        /// <summary>
        /// Calidad del grano producido
        /// </summary>
        public GrainQuality? CalidadGrano { get; set; }

        /// <summary>
        /// Historia u origen de la variedad
        /// </summary>
        public string? Historia { get; set; }

        /// <summary>
        /// Obtentor o creador de la variedad
        /// </summary>
        [StringLength(150, ErrorMessage = "El obtentor no puede exceder 150 caracteres")]
        public string? Obtentor { get; set; }

        /// <summary>
        /// Familia botánica
        /// </summary>
        [StringLength(100, ErrorMessage = "La familia no puede exceder 100 caracteres")]
        public string? Familia { get; set; }

        /// <summary>
        /// Grupo genético al que pertenece
        /// </summary>
        [StringLength(100, ErrorMessage = "El grupo genético no puede exceder 100 caracteres")]
        public string? GrupoGenetico { get; set; }

        /// <summary>
        /// Lista de resistencias de la variedad con sus niveles
        /// </summary>
        public List<CreateVarietyResistanceDto>? Resistencias { get; set; }

        /// <summary>
        /// Lista de imágenes de la variedad
        /// </summary>
        public List<CreateVarietyImageDto>? Imagenes { get; set; }
    }

    /// <summary>
    /// DTO para crear una resistencia de variedad
    /// </summary>
    public class CreateVarietyResistanceDto
    {
        /// <summary>
        /// ID de la resistencia
        /// </summary>
        [Required(ErrorMessage = "El ID de resistencia es obligatorio")]
        public int ResistenciaId { get; set; }

        /// <summary>
        /// Nivel de resistencia de la variedad
        /// </summary>
        [Required(ErrorMessage = "El nivel de resistencia es obligatorio")]
        public ResistanceLevel Nivel { get; set; }
    }

    /// <summary>
    /// DTO para crear una imagen de variedad
    /// </summary>
    public class CreateVarietyImageDto
    {
        /// <summary>
        /// URL de la imagen
        /// </summary>
        [Required(ErrorMessage = "La URL de la imagen es obligatoria")]
        [Url(ErrorMessage = "La URL debe tener un formato válido")]
        [StringLength(255, ErrorMessage = "La URL no puede exceder 255 caracteres")]
        public string UrlImagen { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la imagen
        /// </summary>
        public string? Descripcion { get; set; }        
}