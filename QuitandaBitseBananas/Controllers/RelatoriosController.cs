using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuitandaBitseBananas.Controllers
{
    [Authorize]
    public class RelatoriosController : Controller
    {
        private readonly Data.QuitandaBitseBananasContext _context;
        public RelatoriosController(Data.QuitandaBitseBananasContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> BaixarRelatorioProdutos()
        {
            var produtos = await _context.Produto
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Estoque Atual");

                // Cabeçalhos
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Produto";
                worksheet.Cell(1, 3).Value = "Categoria";
                worksheet.Cell(1, 4).Value = "Fornecedor";
                worksheet.Cell(1, 5).Value = "Preço";
                worksheet.Cell(1, 6).Value = "Estoque";
                worksheet.Cell(1, 7).Value = "Unidade";
                worksheet.Cell(1, 8).Value = "Data Validade";
                worksheet.Cell(1, 9).Value = "Status";

                // Estilzação dos cabeçalhos
                var cabecalhos = worksheet.Range("A1:I1");
                cabecalhos.Style.Font.Bold = true;
                cabecalhos.Style.Fill.BackgroundColor = XLColor.LightGray;

                int linha = 2;
                foreach (var produto in produtos)
                {
                    worksheet.Cell(linha, 1).Value = produto.Id;
                    worksheet.Cell(linha, 2).Value = produto.Name;
                    worksheet.Cell(linha, 3).Value = produto.Categoria?.Name ?? "N/A";
                    worksheet.Cell(linha, 4).Value = produto.Fornecedor?.Name ?? "N/A";
                    worksheet.Cell(linha, 5).Value = produto.Preco;
                    worksheet.Cell(linha, 6).Value = produto.Quantidade;
                    worksheet.Cell(linha, 7).Value = produto.Unidade.ToString();
                    worksheet.Cell(linha, 8).Value = produto.DataValidade.ToString("dd/MM/yyyy");
                    worksheet.Cell(linha, 9).Value = produto.DataValidade < DateTime.Now ? "Vencido" : "Válido";
                    linha++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string nomeArquivo = $"Estoque_Quitanda_{DateTime.Now:dd-MM-yyyy}.xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
                }
            }
        }
    }
}
