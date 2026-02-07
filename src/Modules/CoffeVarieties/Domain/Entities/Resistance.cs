using System.ComponentModel.DataAnnotations;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities
{
    public class Resistance
    {
        /// <summary>
        /// ID único de la resistencia (Primary Key)
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tipo de resistencia: roya, antracnosis, nematodos
        /// Corresponde al enum definido en la base de datos
        /// </summary>
        [Required]
        public ResistanceType Type { get; set; }  // ← Cambiar de Tipo

        // ===== PROPIEDADES DE NAVEGACIÓN =====

        /// <summary>
        /// Colección de relaciones variedad-resistencia
        /// Una resistencia puede estar asociada a múltiples variedades
        /// </summary>
        public virtual ICollection<VarietyResistance> VarietyResistances { get; set; } = new List<VarietyResistance>();

        // ===== PROPIEDADES CALCULADAS =====

        /// <summary>
        /// Obtiene el nombre descriptivo del tipo de resistencia
        /// </summary>
        public string DescriptiveName => Type switch
        {
            ResistanceType.roya => "Roya del Cafeto",
            ResistanceType.antracnosis => "Antracnosis",
            ResistanceType.nematodos => "Nematodos",
            _ => "Resistencia Desconocida"
        };

        /// <summary>
        /// Obtiene una descripción técnica de la resistencia
        /// </summary>
        public string TechnicalDescription => Type switch
        {
            ResistanceType.roya => "Resistencia a Hemileia vastatrix, hongo causante de la roya del cafeto",
            ResistanceType.antracnosis => "Resistencia a Colletotrichum spp., causante de antracnosis en frutos y hojas",
            ResistanceType.nematodos => "Resistencia a nematodos del género Meloidogyne spp.",
            _ => "Sin descripción disponible"
        };

        // ===== MÉTODOS ÚTILES =====

        /// <summary>
        /// Verifica si esta resistencia es crítica para el cultivo
        /// </summary>
        /// <returns>True si la resistencia es considerada crítica</returns>
        public bool IsCriticalResistance()
        {
            return Type == ResistanceType.roya; // La roya es la enfermedad más crítica del café
        }

        /// <summary>
        /// Obtiene el nivel de importancia de la resistencia (1-3, siendo 3 el más importante)
        /// </summary>
        /// <returns>Nivel de importancia de la resistencia</returns>
        public int ImportanceLevel => Type switch
        {
            ResistanceType.roya => 3,       // Crítica
            ResistanceType.antracnosis => 2, // Importante
            ResistanceType.nematodos => 2,   // Importante
            _ => 1
        };

        public override string ToString()
        {
            return DescriptiveName;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Resistance other) return false;
            return Id == other.Id && Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Type);
        }
    }
}