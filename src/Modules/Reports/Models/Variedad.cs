using System.ComponentModel.DataAnnotations.Schema;

namespace pdf.Models
{
    public class Variedad
    {
        public int Id { get; set; }

        [Column("nombre_comun")]
        public string NombreComun { get; set; } = string.Empty;

        [Column("nombre_cientifico")]
        public string? NombreCientifico { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("porte")]
        public string Porte { get; set; } = string.Empty; // alto o bajo

        [Column("tamano_grano")]
        public string TamanoGrano { get; set; } = string.Empty; // pequeño, medio, grande

        [Column("altitud_min")]
        public int AltitudMin { get; set; }

        [Column("altitud_max")]
        public int AltitudMax { get; set; }

        [Column("rendimiento")]
        public string Rendimiento { get; set; } = string.Empty;

        [Column("calidad_grano")]
        public string? CalidadGrano { get; set; }

        [Column("historia")]
        public string? Historia { get; set; }

        [Column("obtentor")]
        public string? Obtentor { get; set; }

        [Column("familia")]
        public string? Familia { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Column("grupo_genetico_id")]
        public int GrupoGeneticoId { get; set; }

        public GrupoGenetico? GrupoGenetico { get; set; }
        public Resistencia? Resistencia { get; set; }
    }
}


