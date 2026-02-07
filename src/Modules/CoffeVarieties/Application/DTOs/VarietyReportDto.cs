using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
    // ===============================
    // REPORTES
    // ===============================

    /// <summary>
    /// DTO para reportes de variedades
    /// </summary>
    public class VarietyReportDto
    {
        /// <summary>
        /// Identificador de la variedad
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre común
        /// </summary>
        public string CommonName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre científico
        /// </summary>
        public string ScientificName { get; set; } = string.Empty;

        /// <summary>
        /// Altura de la planta
        /// </summary>
        public PlantHeight PlantHeight { get; set; }

        /// <summary>
        /// Altura de planta en texto
        /// </summary>
        public string PlantHeightText => PlantHeight switch
        {
            PlantHeight.alto => "Alto",
            PlantHeight.bajo => "Bajo",
            _ => "No especificado"
        };

        /// <summary>
        /// Tamaño de grano
        /// </summary>
        public GrainSize GrainSize { get; set; }

        /// <summary>
        /// Tamaño de grano en texto
        /// </summary>
        public string GrainSizeText => GrainSize switch
        {
            GrainSize.pequeno => "Pequeño",
            GrainSize.medio => "Medio",
            GrainSize.grande => "Grande",
            _ => "No especificado"
        };

        /// <summary>
        /// Potencial de rendimiento
        /// </summary>
        public YieldPotential? YieldPotential { get; set; }

        /// <summary>
        /// Potencial de rendimiento en texto
        /// </summary>
        public string? YieldPotentialText => YieldPotential switch
        {
            Domain.Enums.YieldPotential.muy_bajo => "Muy bajo",
            Domain.Enums.YieldPotential.bajo => "Bajo",
            Domain.Enums.YieldPotential.medio => "Medio",
            Domain.Enums.YieldPotential.alto => "Alto",
            Domain.Enums.YieldPotential.excepcional => "Excepcional",
            _ => null
        };

        /// <summary>
        /// Rango de altitud formateado
        /// </summary>
        public string AltitudeRange { get; set; } = string.Empty;

        /// <summary>
        /// Grupo genético
        /// </summary>
        public string GeneticGroup { get; set; } = string.Empty;

        /// <summary>
        /// Estado activo/inactivo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Estado en texto
        /// </summary>
        public string StatusText => IsActive ? "Activa" : "Inactiva";

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Número de resistencias
        /// </summary>
        public int ResistanceCount { get; set; }

        /// <summary>
        /// Número de imágenes
        /// </summary>
        public int ImageCount { get; set; }
}