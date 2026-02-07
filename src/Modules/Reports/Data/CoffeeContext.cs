using Microsoft.EntityFrameworkCore;
using pdf.Models;

namespace pdf.Data
{
    public class CoffeeContext : DbContext
    {
        public DbSet<GrupoGenetico> GruposGeneticos { get; set; }
        public DbSet<Variedad> Variedades { get; set; }
        public DbSet<Resistencia> Resistencias { get; set; }

        public CoffeeContext(DbContextOptions<CoffeeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

    
            modelBuilder.Entity<Variedad>()
                .HasOne(v => v.GrupoGenetico)
                .WithMany(g => g.Variedades)
                .HasForeignKey(v => v.GrupoGeneticoId);

            modelBuilder.Entity<Variedad>()
                .HasOne(v => v.Resistencia)
                .WithOne(r => r.Variedad)
                .HasForeignKey<Resistencia>(r => r.VariedadId);

            modelBuilder.Entity<GrupoGenetico>().ToTable("grupos_geneticos");
            modelBuilder.Entity<Variedad>().ToTable("variedades");
            modelBuilder.Entity<Resistencia>().ToTable("resistencias");
        }
    }
}
