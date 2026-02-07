using CoffeeApp.src.Modules.CoffeVarieties.Application.Interfaces;
using CoffeeApp.src.Modules.CoffeVarieties.Application.DTOs;
using CoffeeApp.src.Modules.CoffeVarieties.Domain.Enums;

namespace CoffeeApp.src.Modules.CoffeVarieties.UI
{
    /// <summary>
    /// Menú principal del módulo de variedades de café
    /// </summary>
    public class VarietyMainMenu
    {
        public readonly IVarietyService _varietyService;

        public VarietyMainMenu(IVarietyService varietyService)
        {
            _varietyService = varietyService ?? throw new ArgumentNullException(nameof(varietyService));
        }

        public async Task ShowMenuAsync()
        {
            bool continueRunning = true;

            while (continueRunning)
            {
                ShowHeader();
                ShowOptions();

                Console.Write("Seleccione una opción: ");
                var input = Console.ReadLine();

                try
                {
                    switch (input)
                    {
                        case "1":
                            await ShowAllVarietiesAsync();
                            break;
                        case "2":
                            await SearchVarietyAsync();
                            break;
                        case "3":
                            await ShowFilterMenuAsync();
                            break;
                        case "4":
                            await ShowVarietyDetailsAsync();
                            break;
                        case "5":
                            await ShowRecommendationsAsync();
                            break;
                        case "6":
                            await GeneratePdfCatalogAsync();
                            break;
                        case "7":
                            await ShowStatisticsAsync();
                            break;
                        case "0":
                            continueRunning = false;
                            Console.WriteLine("¡Gracias por usar el Catálogo de Variedades de Café Colombiano!");
                            break;
                        default:
                            Console.WriteLine("❌ Opción inválida. Por favor, seleccione una opción del 0-7.");
                            break;
                    }

                    if (continueRunning && input != "0")
                    {
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        public void ShowHeader()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          🌱 CATÁLOGO DE VARIEDADES DE CAFÉ COLOMBIANO 🌱      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        public void ShowOptions()
        {
            Console.WriteLine("📋 MENÚ PRINCIPAL:");
            Console.WriteLine("1. 📚 Ver todas las variedades");
            Console.WriteLine("2. 🔍 Buscar variedad por nombre");
            Console.WriteLine("3. 🎛️  Filtrar variedades");
            Console.WriteLine("4. 📄 Ver ficha técnica completa");
            Console.WriteLine("5. 💡 Sugerencias personalizadas");
            Console.WriteLine("6. 📑 Generar catálogo PDF");
            Console.WriteLine("7. 📊 Ver estadísticas");
            Console.WriteLine("0. ⬅️  Salir");
            Console.WriteLine();
        }

        public async Task ShowAllVarietiesAsync()
        {
            Console.Clear();
            Console.WriteLine("📚 TODAS LAS VARIEDADES DE CAFÉ\n");

            var varieties = await _varietyService.GetAllVarietiesAsync();
            var varietyList = varieties.ToList();

            if (!varietyList.Any())
            {
                Console.WriteLine("❌ No se encontraron variedades registradas.");
                return;
            }

            DisplayVarietiesList(varietyList);
        }

        public async Task SearchVarietyAsync()
        {
            Console.Clear();
            Console.WriteLine("🔍 BÚSQUEDA DE VARIEDADES\n");

            Console.Write("Ingrese el nombre de la variedad a buscar: ");
            var searchTerm = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("❌ Debe ingresar un término de búsqueda.");
                return;
            }

            var varieties = await _varietyService.SearchVarietiesAsync(searchTerm);
            var varietyList = varieties.ToList();

            if (!varietyList.Any())
            {
                Console.WriteLine($"❌ No se encontraron variedades que coincidan con '{searchTerm}'.");
                return;
            }

            Console.WriteLine($"✅ Se encontraron {varietyList.Count} resultado(s) para '{searchTerm}':\n");
            DisplayVarietiesList(varietyList);
        }

        public async Task ShowFilterMenuAsync()
        {
            Console.Clear();
            Console.WriteLine("🎛️ FILTROS DE VARIEDADES\n");

            Console.WriteLine("Seleccione el tipo de filtro:");
            Console.WriteLine("1. Por porte de la planta");
            Console.WriteLine("2. Por tamaño de grano");
            Console.WriteLine("3. Por rango de altitud");
            Console.WriteLine("4. Por potencial de rendimiento");
            Console.WriteLine("5. Filtros combinados");
            Console.WriteLine("0. Volver al menú principal");

            Console.Write("\nSeleccione una opción: ");
            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await FilterByPlantHeightAsync();
                    break;
                case "2":
                    await FilterByGrainSizeAsync();
                    break;
                case "3":
                    await FilterByAltitudeAsync();
                    break;
                case "4":
                    await FilterByYieldPotentialAsync();
                    break;
                case "5":
                    await ShowCombinedFiltersAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    break;
            }
        }

        public async Task ShowVarietyDetailsAsync()
        {
            Console.Clear();
            Console.WriteLine("📄 FICHA TÉCNICA DETALLADA\n");

            // Primero mostrar lista para que elija
            var varieties = await _varietyService.GetAllVarietiesAsync();
            var varietyList = varieties.ToList();

            if (!varietyList.Any())
            {
                Console.WriteLine("❌ No hay variedades registradas.");
                return;
            }

            Console.WriteLine("Variedades disponibles:");
            for (int i = 0; i < varietyList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {varietyList[i].NombreComun}");
            }

            Console.Write($"\nSeleccione una variedad (1-{varietyList.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int selection) && 
                selection > 0 && selection <= varietyList.Count)
            {
                var selectedVariety = varietyList[selection - 1];
                DisplayVarietyDetails(selectedVariety);
            }
            else
            {
                Console.WriteLine("❌ Selección inválida.");
            }
        }

        public async Task ShowRecommendationsAsync()
        {
            Console.Clear();
            Console.WriteLine("💡 SUGERENCIAS PERSONALIZADAS\n");

            Console.WriteLine("¿Para qué tipo de productor busca recomendaciones?");
            Console.WriteLine("1. Agricultor principiante");
            Console.WriteLine("2. Cultivo en alta altitud");
            Console.WriteLine("3. Cultivo en baja altitud");
            Console.WriteLine("4. Alto rendimiento");
            Console.WriteLine("5. Resistentes a enfermedades");

            Console.Write("Seleccione una opción: ");
            var option = Console.ReadLine();

            IEnumerable<VarietyResponseDto> recommendations;

            switch (option)
            {
                case "1":
                    recommendations = await _varietyService.GetVarietiesForBeginnerFarmersAsync();
                    Console.WriteLine("🌱 VARIEDADES RECOMENDADAS PARA PRINCIPIANTES:\n");
                    break;
                case "2":
                    Console.Write("Ingrese la altitud (msnm): ");
                    if (int.TryParse(Console.ReadLine(), out int highAltitude))
                    {
                        recommendations = await _varietyService.GetVarietiesForHighAltitudeAsync(highAltitude);
                        Console.WriteLine($"⛰️ VARIEDADES PARA ALTITUD ALTA ({highAltitude}+ msnm):\n");
                    }
                    else
                    {
                        Console.WriteLine("❌ Altitud inválida.");
                        return;
                    }
                    break;
                case "3":
                    Console.Write("Ingrese la altitud máxima (msnm): ");
                    if (int.TryParse(Console.ReadLine(), out int lowAltitude))
                    {
                        recommendations = await _varietyService.GetVarietiesForLowAltitudeAsync(lowAltitude);
                        Console.WriteLine($"🏞️ VARIEDADES PARA ALTITUD BAJA (hasta {lowAltitude} msnm):\n");
                    }
                    else
                    {
                        Console.WriteLine("❌ Altitud inválida.");
                        return;
                    }
                    break;
                case "4":
                    recommendations = await _varietyService.GetHighYieldVarietiesAsync();
                    Console.WriteLine("📈 VARIEDADES DE ALTO RENDIMIENTO:\n");
                    break;
                case "5":
                    recommendations = await _varietyService.GetDiseaseResistantVarietiesAsync();
                    Console.WriteLine("🛡️ VARIEDADES RESISTENTES A ENFERMEDADES:\n");
                    break;
                default:
                    Console.WriteLine("❌ Opción inválida.");
                    return;
            }

            var recommendationList = recommendations.ToList();
            if (recommendationList.Any())
            {
                DisplayVarietiesList(recommendationList);
            }
            else
            {
                Console.WriteLine("❌ No se encontraron variedades que cumplan los criterios.");
            }
        }

        public async Task GeneratePdfCatalogAsync()
        {
            Console.Clear();
            Console.WriteLine("📑 GENERADOR DE CATÁLOGO PDF\n");

            var varieties = await _varietyService.GetAllVarietiesAsync();
            var varietyList = varieties.ToList();

            if (!varietyList.Any())
            {
                Console.WriteLine("❌ No hay variedades para generar el catálogo.");
                return;
            }

            Console.WriteLine("Seleccione las variedades para incluir en el catálogo:");
            Console.WriteLine("0. Todas las variedades");
            
            for (int i = 0; i < varietyList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {varietyList[i].NombreComun}");
            }

            Console.Write("\nIngrese los números separados por comas (ej: 1,3,5) o 0 para todas: ");
            var input = Console.ReadLine();

            List<int> selectedIds = new();

            if (input == "0")
            {
                selectedIds = varietyList.Select(v => v.Id).ToList();
            }
            else
            {
                var selections = input?.Split(',');
                if (selections != null)
                {
                    foreach (var sel in selections)
                    {
                        if (int.TryParse(sel.Trim(), out int index) && 
                            index > 0 && index <= varietyList.Count)
                        {
                            selectedIds.Add(varietyList[index - 1].Id);
                        }
                    }
                }
            }

            if (selectedIds.Any())
            {
                Console.Write("Ingrese el título del catálogo: ");
                var title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = "Catálogo de Variedades de Café Colombiano";
                }

                var pdfBytes = await _varietyService.GenerateCatalogPdfAsync(selectedIds, title);
                
                // TODO: Guardar el archivo PDF en el sistema
                var fileName = $"catalogo_cafe_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                await File.WriteAllBytesAsync(fileName, pdfBytes);
                
                Console.WriteLine($"✅ Catálogo generado exitosamente: {fileName}");
            }
            else
            {
                Console.WriteLine("❌ No se seleccionaron variedades válidas.");
            }
        }

        public async Task ShowStatisticsAsync()
        {
            Console.Clear();
            Console.WriteLine("📊 ESTADÍSTICAS DE VARIEDADES\n");

            var stats = await _varietyService.GetVarietyStatisticsAsync();

            Console.WriteLine($"📈 Total de variedades registradas: {stats.TotalVarieties}\n");

            Console.WriteLine("📏 Distribución por porte de planta:");
            foreach (var stat in stats.ByPlantHeight)
            {
                Console.WriteLine($"   • {stat.Key}: {stat.Value}");
            }

            Console.WriteLine("\n🌰 Distribución por tamaño de grano:");
            foreach (var stat in stats.ByGrainSize)
            {
                Console.WriteLine($"   • {stat.Key}: {stat.Value}");
            }

            Console.WriteLine("\n📊 Distribución por potencial de rendimiento:");
            foreach (var stat in stats.ByYieldPotential)
            {
                Console.WriteLine($"   • {stat.Key}: {stat.Value}");
            }
        }

        // Métodos auxiliares para filtros específicos
        public async Task FilterByPlantHeightAsync()
        {
            Console.WriteLine("\nSeleccione el porte de planta:");
            Console.WriteLine("0. Bajo");
            Console.WriteLine("1. Alto");

            Console.Write("Opción: ");
            var option = Console.ReadLine();

            PlantHeight? selectedHeight = option switch
            {
                "0" => PlantHeight.bajo,
                "1" => PlantHeight.alto,
                _ => null
            };

            if (selectedHeight.HasValue)
            {
                var varieties = await _varietyService.GetVarietiesByPlantHeightAsync(selectedHeight.Value);
                var varietyList = varieties.ToList();
                
                Console.WriteLine($"\n✅ Variedades con porte {selectedHeight}:\n");
                DisplayVarietiesList(varietyList);
            }
            else
            {
                Console.WriteLine("❌ Opción inválida.");
            }
        }

        public async Task FilterByGrainSizeAsync()
        {
            Console.WriteLine("\nSeleccione el tamaño de grano:");
            Console.WriteLine("1. Pequeño");
            Console.WriteLine("2. Mediano");
            Console.WriteLine("3. Grande");

            Console.Write("Opción: ");
            var option = Console.ReadLine();

            GrainSize? selectedSize = option switch
            {
                "1" => GrainSize.pequeno,
                "2" => GrainSize.medio,
                "3" => GrainSize.grande,
                _ => null
            };

            if (selectedSize.HasValue)
            {
                var varieties = await _varietyService.GetVarietiesByGrainSizeAsync(selectedSize.Value);
                var varietyList = varieties.ToList();
                
                Console.WriteLine($"\n✅ Variedades con grano {selectedSize}:\n");
                DisplayVarietiesList(varietyList);
            }
            else
            {
                Console.WriteLine("❌ Opción inválida.");
            }
        }

        public async Task FilterByAltitudeAsync()
        {
            Console.Write("Ingrese altitud mínima (msnm): ");
            if (!int.TryParse(Console.ReadLine(), out int minAltitude))
            {
                Console.WriteLine("❌ Altitud mínima inválida.");
                return;
            }

            Console.Write("Ingrese altitud máxima (msnm): ");
            if (!int.TryParse(Console.ReadLine(), out int maxAltitude))
            {
                Console.WriteLine("❌ Altitud máxima inválida.");
                return;
            }

            var varieties = await _varietyService.GetVarietiesByAltitudeRangeAsync(minAltitude, maxAltitude);
            var varietyList = varieties.ToList();
            
            Console.WriteLine($"\n✅ Variedades para altitud {minAltitude}-{maxAltitude} msnm:\n");
            DisplayVarietiesList(varietyList);
        }

        public async Task FilterByYieldPotentialAsync()
        {
            Console.WriteLine("\nSeleccione el potencial de rendimiento mínimo:");
            Console.WriteLine("1. Bajo");
            Console.WriteLine("2. Medio");
            Console.WriteLine("3. Alto");

            Console.Write("Opción: ");
            var option = Console.ReadLine();

            YieldPotential? selectedYield = option switch
            {
                "1" => YieldPotential.bajo,
                "2" => YieldPotential.medio,
                "3" => YieldPotential.alto,
                _ => null
            };

            if (selectedYield.HasValue)
            {
                var varieties = await _varietyService.GetVarietiesByYieldPotentialAsync(selectedYield.Value);
                var varietyList = varieties.ToList();
                
                Console.WriteLine($"\n✅ Variedades con rendimiento {selectedYield} o superior:\n");
                DisplayVarietiesList(varietyList);
            }
            else
            {
                Console.WriteLine("❌ Opción inválida.");
            }
        }

        public Task ShowCombinedFiltersAsync()
        {
            Console.WriteLine("🔧 FILTROS COMBINADOS - Próximamente implementado");
            // TODO: Implementar filtros combinados usando VarietyFilterDto
            return Task.CompletedTask;
        }

        // Métodos de visualización
        public void DisplayVarietiesList(List<VarietyResponseDto> varieties)
        {
            if (!varieties.Any())
            {
                Console.WriteLine("❌ No se encontraron variedades.");
                return;
            }

            Console.WriteLine("┌─────┬─────────────────────────┬─────────────┬─────────────┬─────────────────┐");
            Console.WriteLine("│ No. │ Nombre Común            │ Porte       │ Grano       │ Altitud (msnm)  │");
            Console.WriteLine("├─────┼─────────────────────────┼─────────────┼─────────────┼─────────────────┤");

            for (int i = 0; i < varieties.Count; i++)
            {
                var variety = varieties[i];
                var nombre = TruncateString(variety.NombreComun, 22);
                var porte = TruncateString(variety.Porte.ToString() ?? "N/A", 10);
                var grano = TruncateString(variety.TamanoGrano.ToString() ?? "N/A", 10);
                var altitud = GetAltitudeDisplay(variety.AltitudMin, variety.AltitudMax);

                Console.WriteLine($"│ {(i + 1),3} │ {nombre,-23} │ {porte,-11} │ {grano,-11} │ {altitud,-15} │");
            }

            Console.WriteLine("└─────┴─────────────────────────┴─────────────┴─────────────┴─────────────────┘");
            Console.WriteLine($"\nTotal: {varieties.Count} variedades encontradas");
        }

        public void DisplayVarietyDetails(VarietyResponseDto variety)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📄 FICHA TÉCNICA: {variety.NombreComun.ToUpper().PadRight(41)} ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            Console.WriteLine($"🏷️  Nombre común: {variety.NombreComun}");
            if (!string.IsNullOrEmpty(variety.NombreCientifico))
                Console.WriteLine($"🔬 Nombre científico: {variety.NombreCientifico}");

            Console.WriteLine($"📏 Porte: {variety.Porte.ToString() ?? "No especificado"}");
            Console.WriteLine($"🌰 Tamaño de grano: {variety.TamanoGrano.ToString() ?? "No especificado"}");
            
            var altitudDisplay = GetAltitudeDisplay(variety.AltitudMin, variety.AltitudMax);
            if (altitudDisplay != "N/A")
                Console.WriteLine($"⛰️  Altitud óptima: {altitudDisplay}");

            if (variety.Rendimiento.HasValue)
                Console.WriteLine($"📈 Rendimiento: {variety.Rendimiento}");

            if (!string.IsNullOrEmpty(variety.GrupoGenetico))
                Console.WriteLine($"🧬 Grupo genético: {variety.GrupoGenetico}");

            if (!string.IsNullOrEmpty(variety.Familia))
                Console.WriteLine($"👨‍👩‍👧‍👦 Familia: {variety.Familia}");

            if (!string.IsNullOrEmpty(variety.Obtentor))
                Console.WriteLine($"👨‍🔬 Obtentor: {variety.Obtentor}");

            if (variety.Resistencias?.Any() == true)
            {
                Console.WriteLine("\n🛡️ RESISTENCIAS:");
                foreach (var resistencia in variety.Resistencias)
                {
                    Console.WriteLine($"   • {resistencia.Tipo}: {resistencia.Nivel}");
                }
            }

            if (!string.IsNullOrEmpty(variety.Descripcion))
            {
                Console.WriteLine($"\n📝 DESCRIPCIÓN:");
                Console.WriteLine($"   {variety.Descripcion}");
            }

            if (!string.IsNullOrEmpty(variety.Historia))
            {
                Console.WriteLine($"\n📚 HISTORIA:");
                Console.WriteLine($"   {variety.Historia}");
            }

            Console.WriteLine($"\n📅 Fecha de registro: {variety.FechaRegistro:dd/MM/yyyy}");
        }

        public string TruncateString(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
                return "N/A";

            return input.Length <= maxLength ? input : input[..(maxLength - 3)] + "...";
        }

        public string GetAltitudeDisplay(int? minAltitude, int? maxAltitude)
        {
            if (minAltitude.HasValue && maxAltitude.HasValue)
                return $"{minAltitude}-{maxAltitude}";
            
            if (minAltitude.HasValue)
                return $"{minAltitude}+";
            
            if (maxAltitude.HasValue)
                return $"≤{maxAltitude}";
            
            return "N/A";
        }
    }
}