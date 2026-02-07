namespace pdf.Models
{
    public class GrupoGenetico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

    
        public ICollection<Variedad>? Variedades { get; set; }
    }
}
