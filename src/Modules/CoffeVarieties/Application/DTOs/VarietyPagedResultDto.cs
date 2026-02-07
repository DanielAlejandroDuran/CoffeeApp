using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
    // ===============================
    // PAGINACIÓN
    // ===============================

    /// <summary>
    /// DTO para resultados paginados de variedades
    /// </summary>
    public class VarietyPagedResultDto
    {
        /// <summary>
        /// Lista de variedades en la página actual
        /// </summary>
        public IEnumerable<VarietyResponseDto> Varieties { get; set; } = new List<VarietyResponseDto>();

        /// <summary>
        /// Total de registros encontrados
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Número de página actual (base 1)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Tamaño de página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de páginas
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indica si hay página anterior
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indica si hay página siguiente
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Número de registros mostrados en la página actual
        /// </summary>
        public int CurrentPageSize => Varieties.Count();

        /// <summary>
        /// Rango de registros mostrados (ej: "1-10 de 50")
        /// </summary>
        public string DisplayRange => TotalCount > 0 
            ? $"{(PageNumber - 1) * PageSize + 1}-{Math.Min(PageNumber * PageSize, TotalCount)} de {TotalCount}"
            : "0 de 0";      
}