using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities
{
    public class Variety
    {
        public Variety()
        {
            Images = new HashSet<VarietyImage>();
            Resistances = new HashSet<VarietyResistance>();
        }

        // id - int, auto_increment, PRIMARY KEY
        public int Id { get; set; }

        // nombre_comun - varchar(100), NOT NULL
        public string CommonName { get; set; } = string.Empty;

        // nombre_cientifico - varchar(150), NULL
        public string? ScientificName { get; set; }

        // descripcion - text, NULL
        public string? Description { get; set; }

        // porte - enum('alto','bajo'), NOT NULL
        public PlantHeight PlantHeight { get; set; }

        // tamano_grano - enum('pequeño','medio','grande'), NOT NULL  
        public GrainSize GrainSize { get; set; }

        // altitud_min - int, NULL
        public int? AltitudeMin { get; set; }

        // altitud_max - int, NULL
        public int? AltitudeMax { get; set; }

        // rendimiento - enum('muy_bajo','bajo','medio','alto','excepcional'), NULL
        public YieldPotential? YieldPotential { get; set; }

        // calidad_grano - enum('muy_baja','baja','media','alta','muy_alta'), NULL
        public GrainQuality? GrainQuality { get; set; }

        // historia - text, NULL
        public string? History { get; set; }

        // obtentor - varchar(150), NULL
        public string? Breeder { get; set; }

        // familia - varchar(100), NULL
        public string? GeneticFamily { get; set; }

        // grupo_genetico - varchar(100), NULL
        public string? GeneticGroup { get; set; }

        // fecha_registro - timestamp, DEFAULT CURRENT_TIMESTAMP
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        public virtual ICollection<VarietyImage> Images { get; set; } = null!;
        public virtual ICollection<VarietyResistance> Resistances { get; set; } = null!;
    }
}