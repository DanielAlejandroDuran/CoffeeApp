using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
    /// <summary>
    /// DTO para respuesta de consultas de variedades de café
    /// </summary>
    public class VarietyResponseDto
    {
    /// <summary>
    /// Identificador único de la variedad
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre común de la variedad de café
    /// </summary>
    public string NombreComun { get; set; } = string.Empty;

    /// <summary>
    /// Nombre científico de la variedad
    /// </summary>
    public string? NombreCientifico { get; set; }

    /// <summary>
    /// Descripción detallada de la variedad
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Porte o altura de la planta
    /// </summary>
    public PlantHeight Porte { get; set; }

    /// <summary>
    /// Descripción textual del porte
    /// </summary>
    public string PorteTexto => Porte switch
    {
        PlantHeight.alto => "Alto",
        PlantHeight.bajo => "Bajo",
        _ => "No especificado"
    };

    /// <summary>
    /// Tamaño del grano de café
    /// </summary>
    public GrainSize TamanoGrano { get; set; }

    /// <summary>
    /// Descripción textual del tamaño de grano
    /// </summary>
    public string TamanoGranoTexto => TamanoGrano switch
    {
        GrainSize.pequeno => "Pequeño",
        GrainSize.medio => "Medio",
        GrainSize.grande => "Grande",
        _ => "No especificado"
    };

    /// <summary>
    /// Altitud mínima recomendada (metros sobre el nivel del mar)
    /// </summary>
    public int? AltitudMin { get; set; }

    /// <summary>
    /// Altitud máxima recomendada (metros sobre el nivel del mar)
    /// </summary>
    public int? AltitudMax { get; set; }

    /// <summary>
    /// Rango de altitud formateado
    /// </summary>
    public string? RangoAltitud => AltitudMin.HasValue && AltitudMax.HasValue 
        ? $"{AltitudMin} - {AltitudMax} msnm"
        : AltitudMin.HasValue 
            ? $"Desde {AltitudMin} msnm"
            : AltitudMax.HasValue 
                ? $"Hasta {AltitudMax} msnm"
                : null;

    /// <summary>
    /// Potencial de rendimiento de la variedad
    /// </summary>
    public YieldPotential? Rendimiento { get; set; }

    /// <summary>
    /// Descripción textual del rendimiento
    /// </summary>
    public string? RendimientoTexto => Rendimiento switch
    {
        YieldPotential.muy_bajo => "Muy bajo",
        YieldPotential.bajo => "Bajo",
        YieldPotential.medio => "Medio",
        YieldPotential.alto => "Alto",
        YieldPotential.excepcional => "Excepcional",
        _ => null
    };

    /// <summary>
    /// Calidad del grano producido
    /// </summary>
    public GrainQuality? CalidadGrano { get; set; }

    /// <summary>
    /// Descripción textual de la calidad del grano
    /// </summary>
    public string? CalidadGranoTexto => CalidadGrano switch
    {
        GrainQuality.muy_baja => "Muy baja",
        GrainQuality.baja => "Baja",
        GrainQuality.media => "Media",
        GrainQuality.alta => "Alta",
        GrainQuality.muy_alta => "Muy alta",
        _ => null
    };

    /// <summary>
    /// Historia u origen de la variedad
    /// </summary>
    public string? Historia { get; set; }

    /// <summary>
    /// Obtentor o creador de la variedad
    /// </summary>
    public string? Obtentor { get; set; }

    /// <summary>
    /// Familia botánica
    /// </summary>
    public string? Familia { get; set; }

    /// <summary>
    /// Grupo genético al que pertenece
    /// </summary>
    public string? GrupoGenetico { get; set; }

    /// <summary>
    /// Fecha de registro en el sistema
    /// </summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// Lista de resistencias de la variedad
    /// </summary>
    public List<VarietyResistanceResponseDto>? Resistencias { get; set; }

    /// <summary>
    /// Lista de imágenes de la variedad
    /// </summary>
    public List<VarietyImageResponseDto>? Imagenes { get; set; }
}

    /// <summary>
    /// DTO para respuesta de resistencias de variedad
    /// </summary>
    public class VarietyResistanceResponseDto
    {
        /// <summary>
        /// Identificador de la resistencia
        /// </summary>
        public int ResistenciaId { get; set; }

        /// <summary>
        /// Tipo de resistencia
        /// </summary>
        public ResistanceType Tipo { get; set; }

        /// <summary>
        /// Descripción textual del tipo de resistencia
        /// </summary>
        public string TipoTexto => Tipo switch
        {
            ResistanceType.roya => "Roya",
            ResistanceType.antracnosis => "Antracnosis",
            ResistanceType.nematodos => "Nematodos",
            _ => "No especificado"
        };

        /// <summary>
        /// Nivel de resistencia
        /// </summary>
        public ResistanceLevel Nivel { get; set; }

        /// <summary>
        /// Descripción textual del nivel de resistencia
        /// </summary>
        public string NivelTexto => Nivel switch
        {
            ResistanceLevel.susceptible => "Susceptible",
            ResistanceLevel.tolerante => "Tolerante",
            ResistanceLevel.resistente => "Resistente",
            _ => "No especificado"
        };
    }

    /// <summary>
    /// DTO para respuesta de imágenes de variedad
    /// </summary>
    public class VarietyImageResponseDto
    {
        /// <summary>
        /// Identificador de la imagen
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// URL de la imagen
        /// </summary>
        public string UrlImagen { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la imagen
        /// </summary>
        public string? Descripcion { get; set; }        
    }