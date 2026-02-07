using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
    // ===============================
    // COMPARACIÓN
    // ===============================

    /// <summary>
    /// DTO para comparación de variedades
    /// </summary>
    public class VarietyComparisonDto
    {
        /// <summary>
        /// Variedades que se están comparando
        /// </summary>
        public List<VarietyResponseDto> Varieties { get; set; } = new();

        /// <summary>
        /// Datos de comparación organizados por característica
        /// </summary>
        public Dictionary<string, List<string>> ComparisonData { get; set; } = new();

        /// <summary>
        /// Similitudes encontradas entre las variedades
        /// </summary>
        public List<string> Similarities { get; set; } = new();

        /// <summary>
        /// Diferencias encontradas entre las variedades
        /// </summary>
        public List<string> Differences { get; set; } = new();

        /// <summary>
        /// Recomendaciones basadas en la comparación
        /// </summary>
        public List<string> Recommendations { get; set; } = new();

        /// <summary>
        /// Resumen de la comparación
        /// </summary>
        public ComparisonSummaryDto Summary { get; set; } = new();
        
        /// <summary>
        /// Fecha cuando se realizó la comparación
        /// </summary>
        public DateTime ComparisonDate { get; set; }        
}