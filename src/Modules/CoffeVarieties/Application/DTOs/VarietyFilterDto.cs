using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs
{
    /// <summary>
    /// DTO para filtros de búsqueda de variedades
    /// </summary>
    public class VarietyFilterDto
    {
        /// <summary>
        /// Término de búsqueda general
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filtro por altura de planta
        /// </summary>
        public PlantHeight? PlantHeight { get; set; }

        /// <summary>
        /// Filtro por tamaño de grano
        /// </summary>
        public GrainSize? GrainSize { get; set; }

        /// <summary>
        /// Filtro por rendimiento mínimo
        /// </summary>
        public YieldPotential? MinYieldPotential { get; set; }

        /// <summary>
        /// Altitud mínima
        /// </summary>
        public int? MinAltitude { get; set; }

        /// <summary>
        /// Altitud máxima
        /// </summary>
        public int? MaxAltitude { get; set; }

        /// <summary>
        /// Grupo genético
        /// </summary>
        public string? GeneticGroup { get; set; }

        /// <summary>
        /// Familia genética
        /// </summary>
        public string? GeneticFamily { get; set; }

        /// <summary>
        /// Obtentor
        /// </summary>
        public string? Breeder { get; set; }

        /// <summary>
        /// Tipo de resistencia
        /// </summary>
        public ResistanceType? ResistanceType { get; set; }

        /// <summary>
        /// Nivel mínimo de resistencia
        /// </summary>
        public ResistanceLevel? MinResistanceLevel { get; set; }
    }
}