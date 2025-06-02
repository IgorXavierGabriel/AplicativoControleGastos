using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using MeuGerenciadorFinanceiroNovo.Models; // Referência ao namespace Models atualizado

namespace MeuGerenciadorFinanceiroNovo.Utils // O namespace para as utilidades
{
    // Classe para exportar relatórios financeiros para PDF
    public static class PdfExporter
    {
        // Exporta uma lista de transações para um arquivo PDF
        public static string ExportToPdf(List<Transaction> transactions, string title = "Relatório Financeiro")
        {
            // Define a licença para QuestPDF (gratuita para uso não comercial)
            QuestPDF.Settings.License = LicenseType.Community;

            var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = $"relatorio_{currentDateTime}.pdf";
            var filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text(title)
                        .SemiBold().FontSize(16).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(10)
                        .Column(column =>
                        {
                            column.Spacing(5);

                            // Cabeçalho da tabela
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1); // Data
                                    columns.RelativeColumn(1); // Tipo
                                    columns.RelativeColumn(1); // Valor
                                    columns.RelativeColumn(2); // Descrição
                                });

                                table.Header(header =>
                                {
                                    // APLIQUE BorderBottom no Cell() antes do Text()
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Text("Data").SemiBold().FontSize(12);
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Text("Tipo").SemiBold().FontSize(12);
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Text("Valor (R$)").SemiBold().FontSize(12);
                                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Text("Descrição").SemiBold().FontSize(12);
                                });

                                // Linhas da tabela
                                foreach (var transacao in transactions)
                                {
                                    table.Cell().Text(transacao.Date.ToString("yyyy-MM-dd"));
                                    table.Cell().Text(transacao.Type);
                                    table.Cell().Text($"R$ {transacao.Value:F2}");
                                    table.Cell().Text(transacao.Description.Length > 40 ? transacao.Description.Substring(0, 40) + "..." : transacao.Description);
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ").FontSize(10);
                            x.CurrentPageNumber().FontSize(10);
                            x.Span(" de ").FontSize(10);
                            x.TotalPages().FontSize(10);
                        });
                });
            })
            .GeneratePdf(filePath); // Salva o PDF no caminho especificado

            return filePath;
        }
    }
}