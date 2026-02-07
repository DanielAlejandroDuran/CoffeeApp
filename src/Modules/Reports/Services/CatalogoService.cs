using pdf.Models;
using pdf.Repositories;

namespace pdf.Services
{
    public class CatalogoService
    {
        private readonly IVariedadRepository _variedadRepo;

        public CatalogoService(IVariedadRepository variedadRepo)
        {
            _variedadRepo = variedadRepo;
        }

        // Mostrar catálogo completo
        public IEnumerable<Variedad> ObtenerCatalogo()
        {
            return _variedadRepo.ObtenerTodas();
        }

        // Obtener variedad por Id
        public Variedad? ObtenerVariedad(int id)
        {
            return _variedadRepo.ObtenerPorId(id);
        }

        // Filtrar variedades
        public IEnumerable<Variedad> FiltrarVariedades(
            string? porte = null,
            string? tamanoGrano = null,
            int? altitudMin = null,
            int? altitudMax = null,
            string? rendimiento = null,
            string? resistenciaRoya = null,
            string? resistenciaAntracnosis = null,
            string? resistenciaNematodos = null
        )
        {
            return _variedadRepo.Filtrar(
                porte,
                tamanoGrano,
                altitudMin,
                altitudMax,
                rendimiento,
                resistenciaRoya,
                resistenciaAntracnosis,
                resistenciaNematodos
            );
        }
    }
}
