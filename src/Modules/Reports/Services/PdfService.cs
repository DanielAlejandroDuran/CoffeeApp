using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using pdf.Models;

namespace pdf.Services
{
    public class PdfService
    {
        public void GenerarCatalogo(IEnumerable<Variedad> variedades, string rutaArchivo, string titulo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header()
                        .Text(titulo)
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(col =>
                    {
                        foreach (var v in variedades)
                        {
                            col.Item().Border(1).Padding(12).Column(ficha =>
                            {
                                string imagePath = Path.Combine("images", $"{v.NombreComun}.jpg");
                                if (File.Exists(imagePath))
                                {
                                    ficha.Item()
                                         .Height(120)            
                                         .AlignCenter()
                                         .Image(imagePath)
                                         .FitHeight();           
                                }

                                ficha.Item().Text($"Nombre común: {v.NombreComun}")
                                    .FontSize(16).Bold();
                                ficha.Item().Text($"Nombre científico: {v.NombreCientifico}");
                                ficha.Item().Text($"Descripción: {v.Descripcion}");
                                ficha.Item().Text($"Porte: {v.Porte}");
                                ficha.Item().Text($"Tamaño de grano: {v.TamanoGrano}");
                                ficha.Item().Text($"Altitud: {v.AltitudMin} - {v.AltitudMax} msnm");
                                ficha.Item().Text($"Rendimiento: {v.Rendimiento}");
                                ficha.Item().Text($"Calidad del grano: {v.CalidadGrano}");
                                ficha.Item().Text($"Historia: {v.Historia}");
                                ficha.Item().Text($"Grupo genético: {v.GrupoGenetico?.Nombre}");
                                ficha.Item().Text(
                                    $"Resistencias → Roya: {v.Resistencia?.Roya}, Antracnosis: {v.Resistencia?.Antracnosis}, Nematodos: {v.Resistencia?.Nematodos}"
                                );
                            });

                            col.Item().PageBreak();
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            })
            .GeneratePdf(rutaArchivo);
        }
    }
}








