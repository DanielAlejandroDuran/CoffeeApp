using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
    // ===============================
    // ESTADÍSTICAS
    // ===============================

    /// <summary>
    /// DTO para estadísticas generales de variedades
    /// </summary>
    public class VarietyStatisticsDto
    {
        /// <summary>
        /// Total de variedades registradas
        /// </summary>
        public int TotalVarieties { get; set; }

        /// <summary>
        /// Total de variedades activas
        /// </summary>
        public int ActiveVarieties { get; set; }

        /// <summary>
        /// Total de variedades inactivas
        /// </summary>
        public int InactiveVarieties { get; set; }

        /// <summary>
        /// Distribución por altura de planta
        /// </summary>
        public Dictionary<PlantHeight, int> ByPlantHeight { get; set; } = new();

        /// <summary>
        /// Distribución por tamaño de grano
        /// </summary>
        public Dictionary<GrainSize, int> ByGrainSize { get; set; } = new();

        /// <summary>
        /// Distribución por potencial de rendimiento
        /// </summary>
        public Dictionary<YieldPotential, int> ByYieldPotential { get; set; } = new();

        /// <summary>
        /// Distribución por familia genética
        /// </summary>
        public Dictionary<string, int> ByGeneticFamily { get; set; } = new();

        /// <summary>
        /// Porcentaje de variedades activas
        /// </summary>
        public double ActivePercentage => TotalVarieties > 0 ? (double)ActiveVarieties / TotalVarieties * 100 : 0;        
}