using pdf.Data;
using pdf.Repositories;
using pdf.Services;
using pdf.Models;
using Microsoft.EntityFrameworkCore;

namespace pdf.UI
{
    public static class Menu
    {
        private static List<Variedad> seleccionadas = new List<Variedad>();

        public static void Iniciar()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoffeeContext>();
            optionsBuilder.UseMySql(
                "server=localhost;database=coffee_catalog;user=coffee;password=JCAmysql",
                new MySqlServerVersion(new Version(8, 0, 34))
            );

            using var context = new CoffeeContext(optionsBuilder.Options);

            IVariedadRepository repo = new VariedadRepository(context);
            var catalogoService = new CatalogoService(repo);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== 📖 Variedades de Café Colombiano ===");
                Console.WriteLine("1. Ver catálogo completo (PDF)");
                Console.WriteLine("2. Filtrar variedades");
                Console.WriteLine("3. Generar PDF de seleccion");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");
                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        MostrarCatalogo(catalogoService);
                        break;
                    case "2":
                        FiltrarVariedades(catalogoService);
                        break;
                    case "3":
                        GenerarPdfSeleccionadas();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("⚠️ Opción inválida.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void MostrarCatalogo(CatalogoService service)
        {
            var variedades = service.ObtenerCatalogo();
            string archivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CatalogoCompleto.pdf");

            var pdfService = new PdfService();
            pdfService.GenerarCatalogo(variedades, archivo, "Catálogo Completo de Variedades de Café Colombiano");

            Console.WriteLine($"📄 Catálogo completo generado en {archivo}");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = archivo,
                UseShellExecute = true
            });

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }

        private static void FiltrarVariedades(CatalogoService service)
        {
            Console.WriteLine("\n=== Filtros ===");
            Console.Write("Porte (alto/bajo o Enter para omitir): ");
            var porte = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(porte)) porte = null;

            Console.Write("Tamaño de grano (pequeño/medio/grande o Enter): ");
            var grano = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(grano)) grano = null;

            Console.Write("Altitud mínima (Enter para omitir): ");
            int? altMin = int.TryParse(Console.ReadLine(), out int val1) ? val1 : null;

            Console.Write("Altitud máxima (Enter para omitir): ");
            int? altMax = int.TryParse(Console.ReadLine(), out int val2) ? val2 : null;

            Console.Write("Rendimiento (muy bajo/bajo/medio/alto/muy alto/excepcional o Enter): ");
            var rendimiento = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(rendimiento)) rendimiento = null;

            Console.Write("Resistencia a Roya (susceptible/tolerante/resistente o Enter): ");
            var roya = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(roya)) roya = null;

            Console.Write("Resistencia a Antracnosis (susceptible/tolerante/resistente o Enter): ");
            var antracnosis = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(antracnosis)) antracnosis = null;

            Console.Write("Resistencia a Nematodos (susceptible/tolerante/resistente o Enter): ");
            var nematodos = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nematodos)) nematodos = null;

            var resultados = service.FiltrarVariedades(
                porte, grano, altMin, altMax, rendimiento, roya, antracnosis, nematodos
            );

            Console.WriteLine("\n=== Resultados ===");
            foreach (var v in resultados)
            {
                Console.WriteLine($"{v.Id}. {v.NombreComun} - {v.Porte}, Grano: {v.TamanoGrano}, Grupo: {v.GrupoGenetico?.Nombre}");
            }

            if (!resultados.Any())
            {
                Console.WriteLine("⚠️ No se encontraron variedades con esos criterios.");
            }
            else
            {
                Console.Write("\nIngrese el ID de una variedad para ver su ficha técnica o Enter para volver: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int id))
                {
                    var variedad = service.ObtenerVariedad(id);
                    if (variedad != null)
                    {
                        MostrarFichaTecnica(variedad);

                        Console.Write("¿Desea agregar esta variedad a la selección para PDF? (s/n): ");
                        if (Console.ReadLine()?.ToLower() == "s")
                        {
                            seleccionadas.Add(variedad);
                            Console.WriteLine("✅ Variedad agregada a la selección.");
                        }
                    }
                }
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }

        private static void MostrarFichaTecnica(Variedad v)
        {
            Console.Clear();
            Console.WriteLine("=== FICHA TÉCNICA ===");
            Console.WriteLine($"Nombre común: {v.NombreComun}");
            Console.WriteLine($"Nombre científico: {v.NombreCientifico}");
            Console.WriteLine($"Descripción: {v.Descripcion}");
            Console.WriteLine($"Porte: {v.Porte}");
            Console.WriteLine($"Tamaño de grano: {v.TamanoGrano}");
            Console.WriteLine($"Altitud: {v.AltitudMin} - {v.AltitudMax} msnm");
            Console.WriteLine($"Rendimiento: {v.Rendimiento}");
            Console.WriteLine($"Calidad del grano: {v.CalidadGrano}");
            Console.WriteLine($"Historia: {v.Historia}");
            Console.WriteLine($"Grupo genético: {v.GrupoGenetico?.Nombre}");
            Console.WriteLine($"Resistencias → Roya: {v.Resistencia?.Roya}, Antracnosis: {v.Resistencia?.Antracnosis}, Nematodos: {v.Resistencia?.Nematodos}");
        }

        private static void GenerarPdfSeleccionadas()
        {
            Console.WriteLine("\n=== Generar Catálogo PDF con Seleccionadas ===");

            if (!seleccionadas.Any())
            {
                Console.WriteLine("⚠️ No ha seleccionado variedades. Use opción 2 (filtrar) para agregarlas.");
            }
            else
            {
                Console.WriteLine($"🔍 Variedades seleccionadas: {seleccionadas.Count}");
                foreach (var v in seleccionadas)
                {
                    Console.WriteLine($" - {v.NombreComun}");
                }

                
                string archivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "VariedadesSeleccionadas.pdf");

                var pdfService = new PdfService();
                pdfService.GenerarCatalogo(seleccionadas, archivo, "Catálogo de Variedades Seleccionadas");

                Console.WriteLine($"📄 Catálogo PDF generado: {archivo}");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = archivo,
                    UseShellExecute = true
                });
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}



