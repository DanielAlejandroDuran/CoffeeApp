using Microsoft.EntityFrameworkCore;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Interfaces;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;
using CoffeeApp.src.Shared.Context;

#pragma warning disable CS8620, CS8714, CS8602, CS8604, CS8619
#pragma warning restore CS8620
#pragma warning restore CS8714

namespace CoffeeApp.src.Modules.CoffeVarieties.Infraestructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio para variedades de café
    /// </summary>
    public class VarietyRepository : IVarietyRepository
    {
        private readonly AppDbContext _context;

        public VarietyRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ===== CRUD BÁSICO =====

        public async Task<IEnumerable<Variety>> GetAllAsync(bool includeRelations = true)
        {
            var query = _context.Varieties.AsQueryable();

            if (includeRelations)
            {
                query = ApplyIncludes(query);
            }

            return await query
                .OrderBy(v => v.CommonName)
                .ToListAsync();
        }

        public async Task<Variety?> GetByIdAsync(int id, bool includeRelations = true)
        {
            var query = _context.Varieties.AsQueryable();

            if (includeRelations)
            {
                query = ApplyIncludes(query);
            }

            return await query.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Variety> CreateAsync(Variety variety)
        {
            if (variety == null)
                throw new ArgumentNullException(nameof(variety));

            _context.Varieties.Add(variety);
            await _context.SaveChangesAsync();
            return variety;
        }

        public async Task<Variety> UpdateAsync(Variety variety)
        {
            if (variety == null)
                throw new ArgumentNullException(nameof(variety));

            _context.Entry(variety).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return variety;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var variety = await _context.Varieties.FindAsync(id);
            if (variety == null)
                return false;

            _context.Varieties.Remove(variety);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Varieties.AnyAsync(v => v.Id == id);
        }

        // ===== BÚSQUEDAS Y FILTROS =====

        public async Task<IEnumerable<Variety>> GetFilteredAsync(VarietyFilters filters)
        {
            if (filters == null)
                return await GetAllAsync();

            var query = _context.Varieties.AsQueryable();
            query = ApplyIncludes(query);
            // Aplicar filtros básicos
            if (filters.PlantHeight.HasValue)
                query = query.Where(v => v.PlantHeight == filters.PlantHeight.Value);

            if (filters.GrainSize.HasValue)
                query = query.Where(v => v.GrainSize == filters.GrainSize.Value);

            if (filters.MinYieldPotential.HasValue)
                query = query.Where(v => v.YieldPotential >= filters.MinYieldPotential.Value);

            if (filters.MaxYieldPotential.HasValue)
                query = query.Where(v => v.YieldPotential <= filters.MaxYieldPotential.Value);

            if (filters.MinGrainQuality.HasValue)
                query = query.Where(v => v.GrainQuality >= filters.MinGrainQuality.Value);

            if (filters.MaxGrainQuality.HasValue)
                query = query.Where(v => v.GrainQuality <= filters.MaxGrainQuality.Value);

            // Filtros de altitud
            if (filters.MinAltitude.HasValue)
                query = query.Where(v => v.AltitudeMax == null || v.AltitudeMax >= filters.MinAltitude.Value);

            if (filters.MaxAltitude.HasValue)
                query = query.Where(v => v.AltitudeMin == null || v.AltitudeMin <= filters.MaxAltitude.Value);

            // Filtros de información genética
            if (!string.IsNullOrWhiteSpace(filters.GeneticGroup))
                query = query.Where(v => v.GeneticGroup != null &&
                    v.GeneticGroup.ToLower().Contains(filters.GeneticGroup.ToLower()));

            if (!string.IsNullOrWhiteSpace(filters.GeneticFamily))
                query = query.Where(v => v.GeneticFamily != null &&
                    v.GeneticFamily.ToLower().Contains(filters.GeneticFamily.ToLower()));

            if (!string.IsNullOrWhiteSpace(filters.Breeder))
                query = query.Where(v => v.Breeder != null &&
                    v.Breeder.ToLower().Contains(filters.Breeder.ToLower()));

            // Filtros de fecha
            if (filters.CreatedAfter.HasValue)
                query = query.Where(v => v.CreatedAt >= filters.CreatedAfter.Value);

            if (filters.CreatedBefore.HasValue)
                query = query.Where(v => v.CreatedAt <= filters.CreatedBefore.Value);

            // Filtros de resistencia
            if (filters.RequiredResistances?.Any() == true)
            {
                foreach (var requiredResistance in filters.RequiredResistances)
                {
                    query = query.Where(v => v.Resistances != null && v.Resistances.Any(vr =>
                        vr.Resistance != null &&
                        vr.Resistance.Type == requiredResistance.Key &&
                        vr.Level >= requiredResistance.Value));
                }
            }

            return await query
                .OrderBy(v => v.CommonName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Variety>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var lowerSearchTerm = searchTerm.ToLower().Trim();
            var query = _context.Varieties.AsQueryable();
            query = ApplyIncludes(query);

            return await query
                .Where(v =>
                    v.CommonName.ToLower().Contains(lowerSearchTerm) ||
                    (v.ScientificName != null && v.ScientificName.ToLower().Contains(lowerSearchTerm)) ||
                    (v.Description != null && v.Description.ToLower().Contains(lowerSearchTerm)) ||
                    (v.GeneticGroup != null && v.GeneticGroup.ToLower().Contains(lowerSearchTerm)) ||
                    (v.GeneticFamily != null && v.GeneticFamily.ToLower().Contains(lowerSearchTerm)) ||
                    (v.Breeder != null && v.Breeder.ToLower().Contains(lowerSearchTerm))
                )
                .OrderBy(v => v.CommonName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Variety>> GetByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<Variety>();

            var query = _context.Varieties.AsQueryable();
            query = ApplyIncludes(query);

            return await query
                .Where(v => ids.Contains(v.Id))
                .OrderBy(v => v.CommonName)
                .ToListAsync();
        }

        // ===== PAGINACIÓN =====

        public async Task<(IEnumerable<Variety> varieties, int totalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            VarietyFilters? filters = null,
            string? searchTerm = null)
        {
            var query = _context.Varieties.AsQueryable();
            query = ApplyIncludes(query);

            // Aplicar búsqueda de texto
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower().Trim();
                query = query.Where(v =>
                    v.CommonName.ToLower().Contains(lowerSearchTerm) ||
                    (v.ScientificName != null && v.ScientificName.ToLower().Contains(lowerSearchTerm))
                );
            }

            // Aplicar filtros si existen
            if (filters?.HasFilters() == true)
            {
                query = ApplyFiltersToQuery(query, filters);
            }

            // Obtener total
            var totalCount = await query.CountAsync();

            // Aplicar paginación
            var varieties = await query
                .OrderBy(v => v.CommonName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (varieties, totalCount);
        }

        // ===== VALIDACIONES =====

        public async Task<bool> IsCommonNameUniqueAsync(string commonName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(commonName))
                return false;

            var query = _context.Varieties
                .Where(v => v.CommonName.ToLower() == commonName.ToLower().Trim());

            if (excludeId.HasValue)
                query = query.Where(v => v.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsScientificNameUniqueAsync(string scientificName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(scientificName))
                return true; // Scientific name is optional

            var query = _context.Varieties
                .Where(v => v.ScientificName != null &&
                           v.ScientificName.ToLower() == scientificName.ToLower().Trim());

            if (excludeId.HasValue)
                query = query.Where(v => v.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        // ===== ESTADÍSTICAS =====

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Varieties.CountAsync();
        }

        public async Task<Dictionary<PlantHeight, int>> GetStatsByPlantHeightAsync()
        {
            return await _context.Varieties
                .GroupBy(v => v.PlantHeight)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<GrainSize, int>> GetStatsByGrainSizeAsync()
        {
            return await _context.Varieties
                .GroupBy(v => v.GrainSize)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<YieldPotential, int>> GetStatsByYieldPotentialAsync()
        {
            return await _context.Varieties
                .Where(v => v.YieldPotential.HasValue)
                .GroupBy(v => v.YieldPotential!.Value)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // ===== RESISTENCIAS =====

        public async Task<IEnumerable<Variety>> GetByResistanceAsync(
            ResistanceType resistanceType,
            ResistanceLevel minLevel = ResistanceLevel.tolerante)
        {
            var query = _context.Varieties.AsQueryable();
            query = ApplyIncludes(query);

            return await query
                .Where(v => v.Resistances != null && v.Resistances.Any(vr =>
                    vr.Resistance != null &&
                    vr.Resistance.Type == resistanceType &&
                    vr.Level >= minLevel))
                .OrderBy(v => v.CommonName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Variety>> GetByMultipleResistancesAsync(
            Dictionary<ResistanceType, ResistanceLevel> requiredResistances)
        {
            if (requiredResistances == null || !requiredResistances.Any())
                return Enumerable.Empty<Variety>();

            var query = _context.Varieties.AsQueryable();
            query = ApplyIncludes(query);

            foreach (var requiredResistance in requiredResistances)
            {
                query = query.Where(v => v.Resistances != null && v.Resistances.Any(vr =>
                    vr.Resistance != null &&
                    vr.Resistance.Type == requiredResistance.Key &&
                    vr.Level >= requiredResistance.Value));
            }

            return await query
                .OrderBy(v => v.CommonName)
                .ToListAsync();
        }

        // ===== MÉTODOS AUXILIARES =====

#pragma warning disable CS8620 // El argumento no se puede usar para el parámetro debido a las diferencias en la nulabilidad de los tipos de referencia.
        private IQueryable<Variety> ApplyFiltersToQuery(IQueryable<Variety> query, VarietyFilters filters)
        {
            if (filters.PlantHeight.HasValue)
                query = query.Where(v => v.PlantHeight == filters.PlantHeight.Value);

            if (filters.GrainSize.HasValue)
                query = query.Where(v => v.GrainSize == filters.GrainSize.Value);

            if (filters.MinYieldPotential.HasValue)
                query = query.Where(v => v.YieldPotential >= filters.MinYieldPotential.Value);

            if (filters.MaxYieldPotential.HasValue)
                query = query.Where(v => v.YieldPotential <= filters.MaxYieldPotential.Value);

            if (filters.MinAltitude.HasValue)
                query = query.Where(v => v.AltitudeMax == null || v.AltitudeMax >= filters.MinAltitude.Value);

            if (filters.MaxAltitude.HasValue)
                query = query.Where(v => v.AltitudeMin == null || v.AltitudeMin <= filters.MaxAltitude.Value);

            if (!string.IsNullOrWhiteSpace(filters.GeneticGroup))
                query = query.Where(v => v.GeneticGroup != null &&
                    v.GeneticGroup.ToLower().Contains(filters.GeneticGroup.ToLower()));

            return query;
        }

#pragma warning disable CS8620 // El argumento no se puede usar para el parámetro debido a las diferencias en la nulabilidad de los tipos de referencia.
        private static IQueryable<Variety> ApplyIncludes(IQueryable<Variety> query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            // Using string-based includes to avoid nullable reference issues
            return query
                .Include("Images")
                .Include("Resistances")
                .Include("Resistances.Resistance");
        }
    }
#pragma warning restore CS8620
#pragma warning restore CS8714
}
