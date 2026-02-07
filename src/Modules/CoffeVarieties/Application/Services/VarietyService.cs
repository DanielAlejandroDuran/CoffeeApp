using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;
using CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Interfaces;
using System.Text;
using CoffeeApp.src.Modules.CoffeVarieties.Application.Interfaces;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.Services
{
    /// <summary>
    /// Implementación del servicio de variedades con lógica de negocio
    /// </summary>
    public class VarietyService : IVarietyService
    {
        private readonly IVarietyRepository _varietyRepository;

        public VarietyService(IVarietyRepository varietyRepository)
        {
            _varietyRepository = varietyRepository ?? throw new ArgumentNullException(nameof(varietyRepository));
        }

        // ===============================
        // CRUD BÁSICO CON DTOs
        // ===============================

        public async Task<IEnumerable<VarietyResponseDto>> GetAllVarietiesAsync()
        {
            var varieties = await _varietyRepository.GetAllAsync(includeRelations: true);
            return varieties.Select(MapToResponseDto);
        }

        public async Task<VarietyResponseDto?> GetVarietyByIdAsync(int id)
        {
            var variety = await _varietyRepository.GetByIdAsync(id, includeRelations: true);
            return variety != null ? MapToResponseDto(variety) : null;
        }

        public async Task<VarietyResponseDto> CreateVarietyAsync(CreateVarietyDto dto, int createdByUserId)
        {
            // Validaciones de negocio
            if (!await ValidateVarietyDataAsync(dto))
                throw new InvalidOperationException("Los datos de la variedad no son válidos");

            // Verificar nombres únicos
            if (!await IsCommonNameAvailableAsync(dto.NombreComun))
                throw new InvalidOperationException($"El nombre común '{dto.NombreComun}' ya está en uso");

            if (!string.IsNullOrEmpty(dto.NombreCientifico) &&
                !await IsScientificNameAvailableAsync(dto.NombreCientifico))
                throw new InvalidOperationException($"El nombre científico '{dto.NombreCientifico}' ya está en uso");

            // Mapear DTO a entidad
            var variety = MapCreateDtoToEntity(dto);

            // Crear en la base de datos
            var createdVariety = await _varietyRepository.CreateAsync(variety);

            // Obtener la variedad completa con relaciones
            var completeVariety = await _varietyRepository.GetByIdAsync(createdVariety.Id, includeRelations: true);

            return MapToResponseDto(completeVariety!);
        }

        public async Task<VarietyResponseDto> UpdateVarietyAsync(int id, UpdateVarietyDto dto, int updatedByUserId)
        {
            // Verificar que existe
            var existingVariety = await _varietyRepository.GetByIdAsync(id, includeRelations: false);
            if (existingVariety == null)
                throw new InvalidOperationException($"No se encontró la variedad con ID {id}");

            // Validaciones de negocio
            if (!await ValidateVarietyDataAsync(dto))
                throw new InvalidOperationException("Los datos de la variedad no son válidos");

            // Verificar nombres únicos (excluyendo la variedad actual)
            if (!await IsCommonNameAvailableAsync(dto.NombreComun, id))
                throw new InvalidOperationException($"El nombre común '{dto.NombreComun}' ya está en uso");

            if (!string.IsNullOrEmpty(dto.NombreCientifico) &&
                !await IsScientificNameAvailableAsync(dto.NombreCientifico, id))
                throw new InvalidOperationException($"El nombre científico '{dto.NombreCientifico}' ya está en uso");

            // Actualizar propiedades
            UpdateEntityFromDto(existingVariety, dto);

            // Guardar cambios
            var updatedVariety = await _varietyRepository.UpdateAsync(existingVariety);

            // Obtener la variedad completa con relaciones
            var completeVariety = await _varietyRepository.GetByIdAsync(updatedVariety.Id, includeRelations: true);

            return MapToResponseDto(completeVariety!);
        }

        public async Task<bool> DeleteVarietyAsync(int id, int deletedByUserId)
        {
            var exists = await _varietyRepository.ExistsAsync(id);
            if (!exists)
                throw new InvalidOperationException($"No se encontró la variedad con ID {id}");

            return await _varietyRepository.DeleteAsync(id);
        }

        // ===============================
        // BÚSQUEDAS Y FILTROS INTELIGENTES
        // ===============================

        public async Task<IEnumerable<VarietyResponseDto>> SearchVarietiesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllVarietiesAsync();

            var varieties = await _varietyRepository.SearchAsync(searchTerm);
            return varieties.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesByPlantHeightAsync(PlantHeight plantHeight)
        {
            var filters = new VarietyFilters { PlantHeight = plantHeight };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesByGrainSizeAsync(GrainSize grainSize)
        {
            var filters = new VarietyFilters { GrainSize = grainSize };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesByYieldPotentialAsync(YieldPotential yieldPotential)
        {
            var filters = new VarietyFilters { MinYieldPotential = yieldPotential };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesByAltitudeRangeAsync(int minAltitude, int maxAltitude)
        {
            if (!await IsValidAltitudeRangeAsync(minAltitude, maxAltitude))
                throw new ArgumentException("El rango de altitud no es válido");

            var filters = new VarietyFilters
            {
                MinAltitude = minAltitude,
                MaxAltitude = maxAltitude
            };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto);
        }

        // ===============================
        // RECOMENDACIONES INTELIGENTES
        // ===============================

        public async Task<IEnumerable<VarietyResponseDto>> GetRecommendedVarietiesAsync(
            int? targetAltitude = null,
            PlantHeight? preferredHeight = null,
            GrainSize? preferredGrainSize = null,
            YieldPotential? minimumYield = null,
            string? region = null)
        {
            var filters = new VarietyFilters
            {
                PlantHeight = preferredHeight,
                GrainSize = preferredGrainSize,
                MinYieldPotential = minimumYield
            };

            // Si se especifica altitud, agregar rango con tolerancia del 10%
            if (targetAltitude.HasValue)
            {
                var tolerance = (int)(targetAltitude.Value * 0.1);
                filters.MinAltitude = targetAltitude.Value - tolerance;
                filters.MaxAltitude = targetAltitude.Value + tolerance;
            }

            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto)
                          .OrderByDescending(v => CalculateRecommendationScore(v, targetAltitude, preferredHeight, preferredGrainSize, minimumYield));
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesForBeginnerFarmersAsync()
        {
            // Criterios para principiantes: resistentes a enfermedades, alto rendimiento, fácil manejo
            var diseaseResistant = await GetDiseaseResistantVarietiesAsync();
            var highYield = await GetHighYieldVarietiesAsync();

            // Intersección de ambas listas
            var beginnerFriendly = diseaseResistant
                .Where(dr => highYield.Any(hy => hy.Id == dr.Id))
                .OrderBy(v => v.NombreComun);

            return beginnerFriendly;
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetHighYieldVarietiesAsync()
        {
            var filters = new VarietyFilters
            {
                MinYieldPotential = YieldPotential.alto
            };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto)
                          .OrderByDescending(v => v.Rendimiento);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetDiseaseResistantVarietiesAsync()
        {
            var royaResistant = await _varietyRepository.GetByResistanceAsync(
                ResistanceType.roya, ResistanceLevel.tolerante);

            return royaResistant.Select(MapToResponseDto)
                              .OrderBy(v => v.NombreComun);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesForHighAltitudeAsync(int altitude)
        {
            var filters = new VarietyFilters
            {
                MinAltitude = altitude
            };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto)
                          .Where(v => v.AltitudMax == null || v.AltitudMax >= altitude)
                          .OrderBy(v => v.AltitudMin ?? 0);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesForLowAltitudeAsync(int altitude)
        {
            var filters = new VarietyFilters
            {
                MaxAltitude = altitude
            };
            var varieties = await _varietyRepository.GetFilteredAsync(filters);
            return varieties.Select(MapToResponseDto)
                          .Where(v => v.AltitudMin == null || v.AltitudMin <= altitude)
                          .OrderBy(v => v.AltitudMin ?? 0);
        }

        // ===============================
        // GESTIÓN DE CATÁLOGO PDF
        // ===============================

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesForCatalogAsync(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
                return Enumerable.Empty<VarietyResponseDto>();

            var varieties = await _varietyRepository.GetByIdsAsync(selectedIds);
            return varieties.Select(MapToResponseDto)
                          .OrderBy(v => v.NombreComun);
        }

        public async Task<byte[]> GenerateCatalogPdfAsync(List<int> varietyIds, string catalogTitle = "Catálogo de Variedades de Café Colombiano")
        {
            var varieties = await GetVarietiesForCatalogAsync(varietyIds);

            // TODO: Implementar generación de PDF usando la librería elegida
            // Por ahora, retornamos un placeholder
            var content = GenerateCatalogContent(varieties, catalogTitle);
            return Encoding.UTF8.GetBytes(content);
        }

        // ===============================
        // PANEL ADMINISTRATIVO
        // ===============================

        public async Task<IEnumerable<VarietyResponseDto>> GetActiveVarietiesAsync()
        {
            // Asumiendo que todas las variedades en la BD están activas
            return await GetAllVarietiesAsync();
        }

        public IEnumerable<VarietyResponseDto> GetInactiveVarietiesAsync()
        {
            // TODO: Implementar cuando se agregue campo de estado a la entidad
            return Enumerable.Empty<VarietyResponseDto>();
        }

        public async Task<bool> ActivateVarietyAsync(int id, int activatedByUserId)
        {
            // TODO: Implementar cuando se agregue campo de estado a la entidad
            return await Task.FromResult(true);
        }

        public async Task<bool> DeactivateVarietyAsync(int id, int deactivatedByUserId)
        {
            // TODO: Implementar cuando se agregue campo de estado a la entidad
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<VarietyResponseDto>> GetVarietiesByCreatorAsync(int createdByUserId)
        {
            // TODO: Implementar cuando se agregue campo de creador a la entidad
            return await GetAllVarietiesAsync();
        }

        // ===============================
        // VALIDACIONES DE NEGOCIO
        // ===============================

        public async Task<bool> ValidateVarietyDataAsync(CreateVarietyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreComun))
                return false;

            if (dto.AltitudMin.HasValue && dto.AltitudMax.HasValue)
            {
                if (!await IsValidAltitudeRangeAsync(dto.AltitudMin.Value, dto.AltitudMax.Value))
                    return false;
            }

            return true;
        }

        public async Task<bool> ValidateVarietyDataAsync(UpdateVarietyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreComun))
                return false;

            if (dto.AltitudMin.HasValue && dto.AltitudMax.HasValue)
            {
                if (!await IsValidAltitudeRangeAsync(dto.AltitudMin.Value, dto.AltitudMax.Value))
                    return false;
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> IsValidAltitudeRangeAsync(int minAltitude, int maxAltitude)
        {
            if (minAltitude < 0 || maxAltitude < 0)
                return false;

            if (minAltitude >= maxAltitude)
                return false;

            if (maxAltitude > 5000) // Máxima altitud razonable para café
                return false;

            return await Task.FromResult(true);
        }

        public async Task<bool> IsCommonNameAvailableAsync(string commonName, int? excludeId = null)
        {
            return await _varietyRepository.IsCommonNameUniqueAsync(commonName, excludeId);
        }

        public async Task<bool> IsScientificNameAvailableAsync(string scientificName, int? excludeId = null)
        {
            return await _varietyRepository.IsScientificNameUniqueAsync(scientificName, excludeId);
        }

        // ===============================
        // ESTADÍSTICAS Y REPORTES
        // ===============================

        public async Task<VarietyStatisticsDto> GetVarietyStatisticsAsync()
        {
            var totalCount = await _varietyRepository.GetTotalCountAsync();
            var statsByHeight = await _varietyRepository.GetStatsByPlantHeightAsync();
            var statsByGrain = await _varietyRepository.GetStatsByGrainSizeAsync();
            var statsByYield = await _varietyRepository.GetStatsByYieldPotentialAsync();

            return new VarietyStatisticsDto
            {
                TotalVarieties = totalCount,
                ByPlantHeight = statsByHeight,
                ByGrainSize = statsByGrain,
                ByYieldPotential = statsByYield
            };
        }

        public async Task<IEnumerable<VarietyReportDto>> GetVarietyReportAsync()
        {
            var varieties = await GetAllVarietiesAsync();
            return varieties.Select(v => new VarietyReportDto
            {
                Id = v.Id,
                CommonName = v.NombreComun,  // ✅ CORREGIDO
                ScientificName = v.NombreCientifico ?? "",  // ✅ CORREGIDO
                PlantHeight = v.Porte,  // ✅ CORREGIDO
                GrainSize = v.TamanoGrano,  // ✅ CORREGIDO
                YieldPotential = v.Rendimiento,  // ✅ CORREGIDO
                AltitudeRange = v.RangoAltitud ?? "No especificado",  // ✅ CORREGIDO
                GeneticGroup = v.GrupoGenetico ?? "No especificado",  // ✅ CORREGIDO
                IsActive = true, // Asume activo por defecto
                CreatedAt = v.FechaRegistro,
                ResistanceCount = v.Resistencias?.Count ?? 0,
                ImageCount = v.Imagenes?.Count ?? 0
            });
        }

        public async Task<Dictionary<string, int>> GetVarietiesByGeneticFamilyAsync()
        {
            var varieties = await GetAllVarietiesAsync();
            return varieties.Where(v => !string.IsNullOrEmpty(v.Familia))
                           .GroupBy(v => v.Familia!)
                           .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetVarietiesByBreederAsync()
        {
            var varieties = await GetAllVarietiesAsync();
            return varieties.Where(v => !string.IsNullOrEmpty(v.Obtentor))
                           .GroupBy(v => v.Obtentor!)
                           .ToDictionary(g => g.Key, g => g.Count());
        }

        // ===============================
        // PAGINACIÓN
        // ===============================

        public async Task<VarietyPagedResultDto> GetPagedVarietiesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            PlantHeight? plantHeight = null,
            GrainSize? grainSize = null,
            YieldPotential? yieldPotential = null,
            string? geneticGroup = null)
        {
            var filters = new VarietyFilters
            {
                PlantHeight = plantHeight,
                GrainSize = grainSize,
                MinYieldPotential = yieldPotential,
                GeneticGroup = geneticGroup
            };

            var (varieties, totalCount) = await _varietyRepository.GetPagedAsync(
                pageNumber, pageSize, filters, searchTerm);

            var varietyDtos = varieties.Select(MapToResponseDto).ToList();

            return new VarietyPagedResultDto
            {
                Varieties = varietyDtos,  // ✅ CORREGIDO: Cambió de "Items" a "Varieties"
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                // ✅ CORREGIDO: No asignar HasNextPage y HasPreviousPage (se calculan automáticamente)
            };
        }

        // ===============================
        // COMPARACIÓN DE VARIEDADES
        // ===============================

        public async Task<VarietyComparisonDto> CompareVarietiesAsync(List<int> varietyIds)
        {
            if (varietyIds == null || varietyIds.Count < 2)
                throw new ArgumentException("Se requieren al menos 2 variedades para comparar");

            var varieties = await _varietyRepository.GetByIdsAsync(varietyIds);
            var varietyDtos = varieties.Select(MapToResponseDto).ToList();

            return new VarietyComparisonDto
            {
                Varieties = varietyDtos,
                ComparisonDate = DateTime.UtcNow,  // ✅ Asegúrate de agregar esta propiedad a VarietyComparisonDto
                Summary = new ComparisonSummaryDto  // ✅ CORREGIDO
                {
                    VarietyCount = varietyDtos.Count,
                    BestYieldVariety = varietyDtos
                        .Where(v => v.Rendimiento.HasValue)
                        .OrderByDescending(v => v.Rendimiento)
                        .FirstOrDefault()?.NombreComun,
                    CommonCharacteristics = GenerateCommonCharacteristics(varietyDtos)
                }
            };
        }

        // ===============================
        // MÉTODOS PRIVADOS DE MAPEO
        // ===============================

        private VarietyResponseDto MapToResponseDto(Variety variety)
        {
            return new VarietyResponseDto
            {
                Id = variety.Id,
                NombreComun = variety.CommonName,
                NombreCientifico = variety.ScientificName,
                Descripcion = variety.Description,
                Porte = variety.PlantHeight,
                TamanoGrano = variety.GrainSize,
                AltitudMin = variety.AltitudeMin,
                AltitudMax = variety.AltitudeMax,
                Rendimiento = variety.YieldPotential,
                CalidadGrano = variety.GrainQuality,
                Historia = variety.History,
                Obtentor = variety.Breeder,
                Familia = variety.GeneticFamily,
                GrupoGenetico = variety.GeneticGroup,
                FechaRegistro = variety.CreatedAt,
                Resistencias = variety.Resistances?.Select(vr => new VarietyResistanceResponseDto
                {
                    ResistenciaId = vr.ResistanceId,
                    Tipo = vr.Resistance?.Type ?? ResistanceType.roya,
                    Nivel = vr.Level
                }).ToList(),
                Imagenes = variety.Images?.Select(vi => new VarietyImageResponseDto
                {
                    Id = vi.Id,
                    UrlImagen = vi.ImageUrl,
                    Descripcion = vi.Description
                }).ToList()
            };
        }

        private Variety MapCreateDtoToEntity(CreateVarietyDto dto)
        {
            return new Variety
            {
                CommonName = dto.NombreComun,
                ScientificName = dto.NombreCientifico,
                Description = dto.Descripcion,
                PlantHeight = dto.Porte,
                GrainSize = dto.TamanoGrano,
                AltitudeMin = dto.AltitudMin,
                AltitudeMax = dto.AltitudMax,
                YieldPotential = dto.Rendimiento,
                GrainQuality = dto.CalidadGrano,
                History = dto.Historia,
                Breeder = dto.Obtentor,
                GeneticFamily = dto.Familia,
                GeneticGroup = dto.GrupoGenetico,
                CreatedAt = DateTime.UtcNow
            };
        }

        private void UpdateEntityFromDto(Variety entity, UpdateVarietyDto dto)
        {
            entity.CommonName = dto.NombreComun;
            entity.ScientificName = dto.NombreCientifico;
            entity.Description = dto.Descripcion;
            entity.PlantHeight = dto.Porte;
            entity.GrainSize = dto.TamanoGrano;
            entity.AltitudeMin = dto.AltitudMin;
            entity.AltitudeMax = dto.AltitudMax;
            entity.YieldPotential = dto.Rendimiento;
            entity.GrainQuality = dto.CalidadGrano;
            entity.History = dto.Historia;
            entity.Breeder = dto.Obtentor;
            entity.GeneticFamily = dto.Familia;
            entity.GeneticGroup = dto.GrupoGenetico;
        }

        // ===============================
        // MÉTODOS AUXILIARES
        // ===============================

        private int CalculateRecommendationScore(VarietyResponseDto variety, int? targetAltitude,
            PlantHeight? preferredHeight, GrainSize? preferredGrainSize, YieldPotential? minimumYield)
        {
            int score = 0;

            // Puntuación por altitud
            if (targetAltitude.HasValue && variety.AltitudMin.HasValue && variety.AltitudMax.HasValue)
            {
                if (targetAltitude >= variety.AltitudMin && targetAltitude <= variety.AltitudMax)
                    score += 50;
            }

            // Puntuación por porte preferido
            if (preferredHeight.HasValue && variety.Porte == preferredHeight)
                score += 30;

            // Puntuación por tamaño de grano
            if (preferredGrainSize.HasValue && variety.TamanoGrano == preferredGrainSize)
                score += 20;

            // Puntuación por rendimiento
            if (minimumYield.HasValue && variety.Rendimiento >= minimumYield)
                score += 40;

            return score;
        }

        private string GenerateCatalogContent(IEnumerable<VarietyResponseDto> varieties, string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {title}");
            sb.AppendLine($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine();

            foreach (var variety in varieties)
            {
                sb.AppendLine($"## {variety.NombreComun}");
                if (!string.IsNullOrEmpty(variety.NombreCientifico))
                    sb.AppendLine($"**Nombre científico:** {variety.NombreCientifico}");

                sb.AppendLine($"**Porte:** {variety.PorteTexto}");
                sb.AppendLine($"**Tamaño de grano:** {variety.TamanoGranoTexto}");

                if (!string.IsNullOrEmpty(variety.RangoAltitud))
                    sb.AppendLine($"**Altitud:** {variety.RangoAltitud}");

                if (!string.IsNullOrEmpty(variety.RendimientoTexto))
                    sb.AppendLine($"**Rendimiento:** {variety.RendimientoTexto}");

                if (!string.IsNullOrEmpty(variety.Descripcion))
                    sb.AppendLine($"**Descripción:** {variety.Descripcion}");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GenerateComparisonSummary(List<VarietyResponseDto> varieties)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Comparación de variedades:");

            foreach (var variety in varieties)
            {
                sb.AppendLine($"- {variety.NombreComun}: {variety.PorteTexto}, {variety.TamanoGranoTexto}");
            }

            return sb.ToString();
        }

        public Task<IEnumerable<VarietyResponseDto>> GetVarietiesByResistanceAsync(ResistanceType resistanceType, ResistanceLevel minimumLevel = ResistanceLevel.tolerante)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VarietyResponseDto>> GetVarietiesByMultipleFiltersAsync(VarietyFilterDto filters)
        {
            throw new NotImplementedException();
        }
    
    // Agregar este método privado al final de tu VarietyService
    private List<string> GenerateCommonCharacteristics(List<VarietyResponseDto> varieties)
        {
            var characteristics = new List<string>();

            // Verificar porte común
            var commonHeight = varieties.GroupBy(v => v.Porte).Where(g => g.Count() > 1).FirstOrDefault();
            if (commonHeight != null)
                characteristics.Add($"Porte común: {commonHeight.First().PorteTexto}");

            // Verificar tamaño de grano común
            var commonGrainSize = varieties.GroupBy(v => v.TamanoGrano).Where(g => g.Count() > 1).FirstOrDefault();
            if (commonGrainSize != null)
                characteristics.Add($"Tamaño de grano común: {commonGrainSize.First().TamanoGranoTexto}");

            // Verificar familia genética común
            var commonFamily = varieties.Where(v => !string.IsNullOrEmpty(v.Familia))
                                    .GroupBy(v => v.Familia).Where(g => g.Count() > 1).FirstOrDefault();
            if (commonFamily != null)
                characteristics.Add($"Familia común: {commonFamily.Key}");

            return characteristics;
        }

        Task<IEnumerable<VarietyResponseDto>> IVarietyService.GetInactiveVarietiesAsync()
        {
            throw new NotImplementedException();
        }
    }
}