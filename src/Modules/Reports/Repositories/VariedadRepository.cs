using Microsoft.EntityFrameworkCore;
using pdf.Data;
using pdf.Models;

namespace pdf.Repositories
{
    public class VariedadRepository : IVariedadRepository
    {
        private readonly CoffeeContext _context;

        public VariedadRepository(CoffeeContext context)
        {
            _context = context;
        }

        public IEnumerable<Variedad> ObtenerTodas()
        {
            return _context.Variedades
                           .Include(v => v.GrupoGenetico)
                           .Include(v => v.Resistencia)
                           .ToList();
        }

        public Variedad? ObtenerPorId(int id)
        {
            return _context.Variedades
                           .Include(v => v.GrupoGenetico)
                           .Include(v => v.Resistencia)
                           .FirstOrDefault(v => v.Id == id);
        }

        public IEnumerable<Variedad> Filtrar(
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
            var query = _context.Variedades
                                .Include(v => v.GrupoGenetico)
                                .Include(v => v.Resistencia)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(porte))
                query = query.Where(v => v.Porte == porte);

            if (!string.IsNullOrEmpty(tamanoGrano))
                query = query.Where(v => v.TamanoGrano == tamanoGrano);

            if (altitudMin.HasValue)
                query = query.Where(v => v.AltitudMin >= altitudMin.Value);

            if (altitudMax.HasValue)
                query = query.Where(v => v.AltitudMax <= altitudMax.Value);

            if (!string.IsNullOrEmpty(rendimiento))
                query = query.Where(v => v.Rendimiento == rendimiento);

            if (!string.IsNullOrEmpty(resistenciaRoya))
                query = query.Where(v => v.Resistencia!.Roya == resistenciaRoya);

            if (!string.IsNullOrEmpty(resistenciaAntracnosis))
                query = query.Where(v => v.Resistencia!.Antracnosis == resistenciaAntracnosis);

            if (!string.IsNullOrEmpty(resistenciaNematodos))
                query = query.Where(v => v.Resistencia!.Nematodos == resistenciaNematodos);

            return query.ToList();
        }
    }
}
