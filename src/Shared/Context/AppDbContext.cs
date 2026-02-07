using Microsoft.EntityFrameworkCore;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Entities;
using CoffeeApp.src.Modules.User.Domain.Entities;

namespace CoffeeApp.src.Shared.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
            // DbSets para TODAS las tablas de tu BD
            public DbSet<Usuario> Usuarios { get; set; }
            public DbSet<Variety> Varieties { get; set; }
            public DbSet<VarietyImage> VarietyImages { get; set; }
            public DbSet<Resistance> Resistances { get; set; }
            public DbSet<VarietyResistance> VarietyResistances { get; set; }
            
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                    // ===== CONFIGURACIÓN TABLA VARIEDADES =====
                    modelBuilder.Entity<Variety>(entity =>
                    {
                        entity.ToTable("variedades");

                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

                        // MAPEO CORRECTO basado en tu BD:
                        entity.Property(e => e.CommonName).HasColumnName("nombre_comun").HasMaxLength(100).IsRequired();
                        entity.Property(e => e.ScientificName).HasColumnName("nombre_cientifico").HasMaxLength(150);
                        entity.Property(e => e.Description).HasColumnName("descripcion").HasColumnType("text");
                        entity.Property(e => e.PlantHeight).HasColumnName("porte").HasConversion<string>().IsRequired();
                        entity.Property(e => e.GrainSize).HasColumnName("tamano_grano").HasConversion<string>().IsRequired();
                        entity.Property(e => e.AltitudeMin).HasColumnName("altitud_min");
                        entity.Property(e => e.AltitudeMax).HasColumnName("altitud_max");
                        entity.Property(e => e.YieldPotential).HasColumnName("rendimiento").HasConversion<string>();
                        entity.Property(e => e.GrainQuality).HasColumnName("calidad_grano").HasConversion<string>();
                        entity.Property(e => e.History).HasColumnName("historia").HasColumnType("text");
                        entity.Property(e => e.Breeder).HasColumnName("obtentor").HasMaxLength(150);
                        entity.Property(e => e.GeneticFamily).HasColumnName("familia").HasMaxLength(100);
                        entity.Property(e => e.GeneticGroup).HasColumnName("grupo_genetico").HasMaxLength(100);
                        entity.Property(e => e.CreatedAt).HasColumnName("fecha_registro").HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
                        entity.HasIndex(e => e.CommonName);
                        entity.HasIndex(e => e.GeneticFamily);

                        entity.HasMany(v => v.Resistances)
                        .WithOne(vr => vr.Variety)
                        .HasForeignKey(vr => vr.VarietyId)
                        .OnDelete(DeleteBehavior.Cascade);
                    });

                    // ===== CONFIGURACIÓN TABLA IMAGENES_VARIEDAD =====
                    modelBuilder.Entity<VarietyImage>(entity =>
                    {
                        entity.ToTable("imagenes_variedad");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).HasColumnName("id");
                        entity.Property(e => e.VarietyId)
                            .HasColumnName("variedad_id")
                            .IsRequired();
                        entity.Property(e => e.ImageUrl)
                            .HasColumnName("url_imagen")
                            .HasMaxLength(255)
                            .IsRequired();
                        entity.Property(e => e.Description)
                            .HasColumnName("descripcion")
                            .HasColumnType("text");

                        // Relación con Variety
                        entity.HasOne(vi => vi.Variety)
                            .WithMany(v => v.Images)
                            .HasForeignKey(vi => vi.VarietyId)
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                    // ===== CONFIGURACIÓN TABLA RESISTENCIAS =====
                    modelBuilder.Entity<Resistance>(entity =>
                    {
                        entity.ToTable("resistencias");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                        entity.Property(e => e.Type).HasColumnName("tipo").HasConversion<string>().IsRequired();
                        
                        // Relación con VarietyResistances
                        entity.HasMany(r => r.VarietyResistances)
                            .WithOne(vr => vr.Resistance)
                            .HasForeignKey(vr => vr.ResistanceId);
                    });
                    

                    // ===== CONFIGURACIÓN TABLA VARIEDADES_RESISTENCIAS =====
                    modelBuilder.Entity<VarietyResistance>(entity =>
                    {
                        entity.ToTable("variedades_resistencias");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                        entity.Property(e => e.VarietyId).HasColumnName("variedad_id").IsRequired();
                        entity.Property(e => e.ResistanceId).HasColumnName("resistencia_id").IsRequired();
                        entity.Property(e => e.Level).HasColumnName("nivel").HasConversion<string>().IsRequired();

                        // Relaciones
                        entity.HasOne(vr => vr.Variety)
                            .WithMany(v => v.Resistances)
                            .HasForeignKey(vr => vr.VarietyId)
                            .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(vr => vr.Resistance)
                            .WithMany(r => r.VarietyResistances)
                            .HasForeignKey(vr => vr.ResistanceId)
                            .OnDelete(DeleteBehavior.Cascade);
                    });

                    // Aplicar configuraciones adicionales si existen
                    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
                }

                // Override SaveChanges para manejar automáticamente fechas de actualización
                public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
                {
                    var entries = ChangeTracker
                        .Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

                    foreach (var entry in entries)
                    {
                        // Manejar UpdatedAt para entidades que lo tengan
                        if (entry.Entity is Variety variety && entry.State == EntityState.Modified)
                        {
                            // Si tu clase Variety tiene UpdatedAt, descomenta esta línea
                            // variety.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    return await base.SaveChangesAsync(cancellationToken);
                }

                // Configuración de conexión
                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    if (!optionsBuilder.IsConfigured)
                    {
                        // Esta configuración debe coincidir con tu appsettings.json
                        string connectionString = "Server=localhost;Database=colombian_coffee_db;User=root;Password=daniel123;";
                        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                        
                        // Solo para desarrollo
                        optionsBuilder.EnableSensitiveDataLogging();
                        optionsBuilder.LogTo(Console.WriteLine);
                    }
        }
    }
}