using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities
{
    /// <summary>
    /// Entidad que representa la relación entre variedades de café y sus resistencias
    /// Mapea a la tabla 'variedades_resistencias' en la base de datos
    /// Esta es una tabla intermedia que establece una relación muchos a muchos
    /// </summary>
    public class VarietyResistance
    {
        /// <summary>
        /// ID único de la relación (Primary Key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID de la variedad de café (Foreign Key)
        /// </summary>
        public int VarietyId { get; set; } // Cambio: era VariedadId

        /// <summary>
        /// ID del tipo de resistencia (Foreign Key)
        /// </summary>
        public int ResistanceId { get; set; } // Cambio: era ResistenciaId

        /// <summary>
        /// Nivel de resistencia: susceptible, tolerante, resistente
        /// </summary>
        public ResistanceLevel Level { get; set; } // Cambio: era Nivel

        // ===== PROPIEDADES DE NAVEGACIÓN =====

        /// <summary>
        /// Variedad de café asociada a esta resistencia
        /// </summary>
        public virtual Variety Variety { get; set; } = null!;

        /// <summary>
        /// Tipo de resistencia asociada a esta variedad
        /// </summary>
        public virtual Resistance Resistance { get; set; } = null!;

        // ===== PROPIEDADES CALCULADAS =====

        /// <summary>
        /// Obtiene el nombre descriptivo del nivel de resistencia
        /// </summary>
        public string LevelDescription => Level switch
        {
            ResistanceLevel.susceptible => "Susceptible",
            ResistanceLevel.tolerante => "Tolerante", 
            ResistanceLevel.resistente => "Resistente",
            _ => "Nivel Desconocido"
        };

        /// <summary>
        /// Obtiene una descripción detallada del nivel de resistencia
        /// </summary>
        public string DetailedDescription => Level switch
        {
            ResistanceLevel.susceptible => "La variedad es vulnerable a esta enfermedad y puede sufrir daños significativos",
            ResistanceLevel.tolerante => "La variedad puede tolerar la enfermedad con daños mínimos bajo ciertas condiciones",
            ResistanceLevel.resistente => "La variedad tiene alta resistencia y raramente se ve afectada por esta enfermedad",
            _ => "Nivel de resistencia no definido"
        };

        /// <summary>
        /// Obtiene el valor numérico del nivel de resistencia (1-3, siendo 3 el más resistente)
        /// </summary>
        public int NumericValue => Level switch
        {
            ResistanceLevel.susceptible => 1,
            ResistanceLevel.tolerante => 2,
            ResistanceLevel.resistente => 3,
            _ => 0
        };

        /// <summary>
        /// Obtiene el color representativo del nivel de resistencia (útil para UI)
        /// </summary>
        public string LevelColor => Level switch
        {
            ResistanceLevel.susceptible => "#FF4444", // Rojo
            ResistanceLevel.tolerante => "#FFA500",   // Naranja
            ResistanceLevel.resistente => "#44AA44",  // Verde
            _ => "#CCCCCC" // Gris
        };

        /// <summary>
        /// Obtiene el icono representativo del nivel (útil para UI)
        /// </summary>
        public string LevelIcon => Level switch
        {
            ResistanceLevel.susceptible => "⚠️",
            ResistanceLevel.tolerante => "🟡",
            ResistanceLevel.resistente => "✅",
            _ => "❓"
        };

        // ===== MÉTODOS ÚTILES =====

        /// <summary>
        /// Verifica si el nivel de resistencia es suficiente para cultivo comercial
        /// </summary>
        /// <returns>True si el nivel es tolerante o resistente</returns>
        public bool IsCommerciallyViable()
        {
            return Level == ResistanceLevel.tolerante || Level == ResistanceLevel.resistente;
        }

        /// <summary>
        /// Verifica si esta resistencia es crítica para la variedad
        /// </summary>
        /// <returns>True si la resistencia es crítica y el nivel es bajo</returns>
        public bool RequiresSpecialAttention()
        {
            return Resistance?.IsCriticalResistance() == true && 
                   Level == ResistanceLevel.susceptible;
        }

        /// <summary>
        /// Obtiene una evaluación general de la resistencia
        /// </summary>
        /// <returns>Texto con la evaluación</returns>
        public string GetEvaluation()
        {
            var resistanceType = Resistance?.DescriptiveName ?? "Resistencia";
            var evaluation = Level switch
            {
                ResistanceLevel.resistente => "Excelente",
                ResistanceLevel.tolerante => "Buena",
                ResistanceLevel.susceptible => "Requiere manejo cuidadoso",
                _ => "No evaluada"
            };

            return $"{resistanceType}: {evaluation}";
        }

        /// <summary>
        /// Compara el nivel de resistencia con otro
        /// </summary>
        /// <param name="other">Otra relación variedad-resistencia</param>
        /// <returns>-1 si es menor, 0 si es igual, 1 si es mayor</returns>
        public int CompareLevelWith(VarietyResistance other)
        {
            return NumericValue.CompareTo(other.NumericValue);
        }

        /// <summary>
        /// Crea una nueva relación variedad-resistencia
        /// </summary>
        /// <param name="varietyId">ID de la variedad</param>
        /// <param name="resistanceId">ID de la resistencia</param>
        /// <param name="level">Nivel de resistencia</param>
        /// <returns>Nueva instancia de VarietyResistance</returns>
        public static VarietyResistance CreateNew(int varietyId, int resistanceId, ResistanceLevel level)
        {
            return new VarietyResistance
            {
                VarietyId = varietyId,
                ResistanceId = resistanceId,
                Level = level
            };
        }

        public override string ToString()
        {
            var varietyName = Variety?.CommonName ?? $"Variedad {VarietyId}";
            var resistanceName = Resistance?.DescriptiveName ?? $"Resistencia {ResistanceId}";
            return $"{varietyName} - {resistanceName}: {LevelDescription}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not VarietyResistance other) return false;
            return Id == other.Id && 
                   VarietyId == other.VarietyId && 
                   ResistanceId == other.ResistanceId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, VarietyId, ResistanceId, Level);
        }
    }
}