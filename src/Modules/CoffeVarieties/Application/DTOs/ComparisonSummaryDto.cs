using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;

    /// <summary>
    /// DTO para resumen de comparación
    /// </summary>
    public class ComparisonSummaryDto
    {
        /// <summary>
        /// Número de variedades comparadas
        /// </summary>
        public int VarietyCount { get; set; }

        /// <summary>
        /// Variedad con mejor rendimiento
        /// </summary>
        public string? BestYieldVariety { get; set; }

        /// <summary>
        /// Variedad más resistente
        /// </summary>
        public string? MostResistantVariety { get; set; }

        /// <summary>
        /// Rango de altitud común
        /// </summary>
        public string? CommonAltitudeRange { get; set; }

        /// <summary>
        /// Características más comunes
        /// </summary>
        public List<string> CommonCharacteristics { get; set; } = new();
    }
