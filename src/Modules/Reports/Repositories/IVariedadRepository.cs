using pdf.Models;

namespace pdf.Repositories
{
    public interface IVariedadRepository
    {
        IEnumerable<Variedad> ObtenerTodas();
        Variedad? ObtenerPorId(int id);
        IEnumerable<Variedad> Filtrar(
            string? porte = null,
            string? tamanoGrano = null,
            int? altitudMin = null,
            int? altitudMax = null,
            string? rendimiento = null,
            string? resistenciaRoya = null,
            string? resistenciaAntracnosis = null,
            string? resistenciaNematodos = null
        );
    }
}
