using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Domain.Interfaces
{
    /// <summary>
    /// Repositorio para el manejo de variedades de café
    /// </summary>
    public interface IVarietyRepository
    {
        // ===== CRUD BÁSICO =====
        
        /// <summary>
        /// Obtiene todas las variedades con sus relaciones
        /// </summary>
        Task<IEnumerable<Variety>> GetAllAsync(bool includeRelations = true);
        
        /// <summary>
        /// Obtiene una variedad por ID
        /// </summary>
        Task<Variety?> GetByIdAsync(int id, bool includeRelations = true);
        
        /// <summary>
        /// Crea una nueva variedad
        /// </summary>
        Task<Variety> CreateAsync(Variety variety);
        
        /// <summary>
        /// Actualiza una variedad existente
        /// </summary>
        Task<Variety> UpdateAsync(Variety variety);
        
        /// <summary>
        /// Elimina una variedad por ID
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Verifica si existe una variedad
        /// </summary>
        Task<bool> ExistsAsync(int id);

        // ===== BÚSQUEDAS Y FILTROS =====
        
        /// <summary>
        /// Obtiene variedades aplicando filtros múltiples
        /// </summary>
        Task<IEnumerable<Variety>> GetFilteredAsync(VarietyFilters filters);
        
        /// <summary>
        /// Búsqueda de texto libre en nombre común, científico y descripción
        /// </summary>
        Task<IEnumerable<Variety>> SearchAsync(string searchTerm);
        
        /// <summary>
        /// Obtiene variedades por una lista de IDs específicos
        /// </summary>
        Task<IEnumerable<Variety>> GetByIdsAsync(List<int> ids);

        // ===== PAGINACIÓN =====
        
        /// <summary>
        /// Obtiene variedades paginadas con filtros opcionales
        /// </summary>
        Task<(IEnumerable<Variety> varieties, int totalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            VarietyFilters? filters = null,
            string? searchTerm = null
        );

        // ===== VALIDACIONES DE NEGOCIO =====
        
        /// <summary>
        /// Verifica si el nombre común es único
        /// </summary>
        Task<bool> IsCommonNameUniqueAsync(string commonName, int? excludeId = null);
        
        /// <summary>
        /// Verifica si el nombre científico es único
        /// </summary>
        Task<bool> IsScientificNameUniqueAsync(string scientificName, int? excludeId = null);

        // ===== ESTADÍSTICAS =====
        
        /// <summary>
        /// Obtiene el total de variedades registradas
        /// </summary>
        Task<int> GetTotalCountAsync();
        
        /// <summary>
        /// Obtiene estadísticas por porte de planta
        /// </summary>
        Task<Dictionary<PlantHeight, int>> GetStatsByPlantHeightAsync();
        
        /// <summary>
        /// Obtiene estadísticas por tamaño de grano
        /// </summary>
        Task<Dictionary<GrainSize, int>> GetStatsByGrainSizeAsync();
        
        /// <summary>
        /// Obtiene estadísticas por potencial de rendimiento
        /// </summary>
        Task<Dictionary<YieldPotential, int>> GetStatsByYieldPotentialAsync();

        // ===== RESISTENCIAS =====
        
        /// <summary>
        /// Obtiene variedades que tienen resistencia a enfermedades específicas
        /// </summary>
        Task<IEnumerable<Variety>> GetByResistanceAsync(
            ResistanceType resistanceType, 
            ResistanceLevel minLevel = ResistanceLevel.tolerante
        );
        
        /// <summary>
        /// Obtiene variedades con múltiples resistencias
        /// </summary>
        Task<IEnumerable<Variety>> GetByMultipleResistancesAsync(
            Dictionary<ResistanceType, ResistanceLevel> requiredResistances
        );
    }

    /// <summary>
    /// Clase para filtros de variedades
    /// </summary>
    public class VarietyFilters
    {
        public PlantHeight? PlantHeight { get; set; }
        public GrainSize? GrainSize { get; set; }
        public YieldPotential? MinYieldPotential { get; set; }
        public YieldPotential? MaxYieldPotential { get; set; }
        public GrainQuality? MinGrainQuality { get; set; }
        public GrainQuality? MaxGrainQuality { get; set; }
        public int? MinAltitude { get; set; }
        public int? MaxAltitude { get; set; }
        public string? GeneticGroup { get; set; }
        public string? GeneticFamily { get; set; }
        public string? Breeder { get; set; }
        
        // Filtros de resistencia
        public Dictionary<ResistanceType, ResistanceLevel>? RequiredResistances { get; set; }
        
        // Filtros de fecha
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        
        /// <summary>
        /// Verifica si hay algún filtro aplicado
        /// </summary>
        public bool HasFilters()
        {
            return PlantHeight.HasValue ||
                   GrainSize.HasValue ||
                   MinYieldPotential.HasValue ||
                   MaxYieldPotential.HasValue ||
                   MinGrainQuality.HasValue ||
                   MaxGrainQuality.HasValue ||
                   MinAltitude.HasValue ||
                   MaxAltitude.HasValue ||
                   !string.IsNullOrEmpty(GeneticGroup) ||
                   !string.IsNullOrEmpty(GeneticFamily) ||
                   !string.IsNullOrEmpty(Breeder) ||
                   (RequiredResistances?.Any() == true) ||
                   CreatedAfter.HasValue ||
                   CreatedBefore.HasValue;
        }
    }
}