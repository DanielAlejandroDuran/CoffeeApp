using CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.Interfaces;
/// <summary>
/// Interfaz del servicio de variedades con lógica de negocio
/// </summary>
public interface IVarietyService
{
    // ===============================
    // CRUD BÁSICO CON DTOs
    // ===============================
    
    /// <summary>
    /// Obtiene todas las variedades activas
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetAllVarietiesAsync();

    /// <summary>
    /// Obtiene una variedad por su ID
    /// </summary>
    Task<VarietyResponseDto?> GetVarietyByIdAsync(int id);

    /// <summary>
    /// Crea una nueva variedad
    /// </summary>
    Task<VarietyResponseDto> CreateVarietyAsync(CreateVarietyDto dto, int createdByUserId);

    /// <summary>
    /// Actualiza una variedad existente
    /// </summary>
    Task<VarietyResponseDto> UpdateVarietyAsync(int id, UpdateVarietyDto dto, int updatedByUserId);

    /// <summary>
    /// Elimina una variedad (soft delete)
    /// </summary>
    Task<bool> DeleteVarietyAsync(int id, int deletedByUserId);

    // ===============================
    // BÚSQUEDAS Y FILTROS INTELIGENTES
    // ===============================

    /// <summary>
    /// Busca variedades por término de búsqueda
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> SearchVarietiesAsync(string searchTerm);

    /// <summary>
    /// Filtra variedades por altura de planta
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByPlantHeightAsync(PlantHeight plantHeight);

    /// <summary>
    /// Filtra variedades por tamaño de grano
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByGrainSizeAsync(GrainSize grainSize);

    /// <summary>
    /// Filtra variedades por potencial de rendimiento
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByYieldPotentialAsync(YieldPotential yieldPotential);

    /// <summary>
    /// Filtra variedades por rango de altitud
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByAltitudeRangeAsync(int minAltitude, int maxAltitude);

    /// <summary>
    /// Filtra variedades por tipo y nivel de resistencia
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByResistanceAsync(
        ResistanceType resistanceType, 
        ResistanceLevel minimumLevel = ResistanceLevel.tolerante
    );

    /// <summary>
    /// Búsqueda avanzada con múltiples filtros combinados
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByMultipleFiltersAsync(VarietyFilterDto filters);

    // ===============================
    // RECOMENDACIONES INTELIGENTES
    // ===============================

    /// <summary>
    /// Obtiene variedades recomendadas según criterios específicos
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetRecommendedVarietiesAsync(
        int? targetAltitude = null,
        PlantHeight? preferredHeight = null,
        GrainSize? preferredGrainSize = null,
        YieldPotential? minimumYield = null,
        string? region = null
    );

    /// <summary>
    /// Variedades recomendadas para agricultores principiantes
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesForBeginnerFarmersAsync();

    /// <summary>
    /// Variedades de alto rendimiento
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetHighYieldVarietiesAsync();

    /// <summary>
    /// Variedades resistentes a enfermedades
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetDiseaseResistantVarietiesAsync();

    /// <summary>
    /// Variedades para altitudes altas
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesForHighAltitudeAsync(int altitude);

    /// <summary>
    /// Variedades para altitudes bajas
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesForLowAltitudeAsync(int altitude);

    // ===============================
    // GESTIÓN DE CATÁLOGO PDF
    // ===============================

    /// <summary>
    /// Obtiene variedades seleccionadas para catálogo
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesForCatalogAsync(List<int> selectedIds);

    /// <summary>
    /// Genera catálogo PDF con variedades seleccionadas
    /// </summary>
    Task<byte[]> GenerateCatalogPdfAsync(List<int> varietyIds, string catalogTitle = "Catálogo de Variedades de Café Colombiano");

    // ===============================
    // PANEL ADMINISTRATIVO
    // ===============================

    /// <summary>
    /// Obtiene variedades activas
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetActiveVarietiesAsync();

    /// <summary>
    /// Obtiene variedades inactivas
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetInactiveVarietiesAsync();

    /// <summary>
    /// Activa una variedad
    /// </summary>
    Task<bool> ActivateVarietyAsync(int id, int activatedByUserId);

    /// <summary>
    /// Desactiva una variedad
    /// </summary>
    Task<bool> DeactivateVarietyAsync(int id, int deactivatedByUserId);

    /// <summary>
    /// Obtiene variedades creadas por un usuario específico
    /// </summary>
    Task<IEnumerable<VarietyResponseDto>> GetVarietiesByCreatorAsync(int createdByUserId);

    // ===============================
    // VALIDACIONES DE NEGOCIO
    // ===============================

    /// <summary>
    /// Valida los datos de una variedad antes de crear/actualizar
    /// </summary>
    Task<bool> ValidateVarietyDataAsync(CreateVarietyDto dto);

    /// <summary>
    /// Valida los datos de una variedad antes de actualizar
    /// </summary>
    Task<bool> ValidateVarietyDataAsync(UpdateVarietyDto dto);

    /// <summary>
    /// Valida si el rango de altitud es válido
    /// </summary>
    Task<bool> IsValidAltitudeRangeAsync(int minAltitude, int maxAltitude);

    /// <summary>
    /// Verifica si el nombre común está disponible
    /// </summary>
    Task<bool> IsCommonNameAvailableAsync(string commonName, int? excludeId = null);

    /// <summary>
    /// Verifica si el nombre científico está disponible
    /// </summary>
    Task<bool> IsScientificNameAvailableAsync(string scientificName, int? excludeId = null);

    // ===============================
    // ESTADÍSTICAS Y REPORTES
    // ===============================

    /// <summary>
    /// Obtiene estadísticas generales de variedades
    /// </summary>
    Task<VarietyStatisticsDto> GetVarietyStatisticsAsync();

    /// <summary>
    /// Genera reporte de variedades
    /// </summary>
    Task<IEnumerable<VarietyReportDto>> GetVarietyReportAsync();

    /// <summary>
    /// Obtiene conteo de variedades por familia genética
    /// </summary>
    Task<Dictionary<string, int>> GetVarietiesByGeneticFamilyAsync();

    /// <summary>
    /// Obtiene conteo de variedades por obtentor
    /// </summary>
    Task<Dictionary<string, int>> GetVarietiesByBreederAsync();

    // ===============================
    // PAGINACIÓN
    // ===============================

    /// <summary>
    /// Obtiene variedades paginadas con filtros
    /// </summary>
    Task<VarietyPagedResultDto> GetPagedVarietiesAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        PlantHeight? plantHeight = null,
        GrainSize? grainSize = null,
        YieldPotential? yieldPotential = null,
        string? geneticGroup = null
    );

    // ===============================
    // COMPARACIÓN DE VARIEDADES
    // ===============================

    /// <summary>
    /// Compara múltiples variedades lado a lado
    /// </summary>
    Task<VarietyComparisonDto> CompareVarietiesAsync(List<int> varietyIds);
}
    