using System.ComponentModel.DataAnnotations.Schema;

namespace pdf.Models
{
    public class Resistencia
    {
        public int Id { get; set; }

        [Column("variedad_id")]
        public int VariedadId { get; set; }
        public Variedad? Variedad { get; set; }

        [Column("roya")]
        public string Roya { get; set; } = string.Empty;

        [Column("antracnosis")]
        public string Antracnosis { get; set; } = string.Empty;

        [Column("nematodos")]
        public string Nematodos { get; set; } = string.Empty;
    }
}

