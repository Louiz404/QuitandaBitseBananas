using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuitandaBitseBananas.Controllers.Api
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardApiController : Controller
    {
        private readonly Data.QuitandaBitseBananasContext _context;
        public DashboardApiController(Data.QuitandaBitseBananasContext context)
        {
            _context = context;
        }

        // GET: api/DashboardApi/Estatisticas
        [HttpGet("Estatisticas")]
        public async Task<IActionResult> GetEstatisticas()
        {
            // Busca dados e retorna como JSON
            var totalProdutos = await _context.Produto.CountAsync();
            var valorEstoque = await _context.Produto.SumAsync(p => p.ValorTotal);
            
            var vencidos = await _context.Produto
                .Where(p => p.DataValidade < DateTime.Now)
                .CountAsync();
            
            var estoqueBaixo = await _context.Produto
                .Where(p => p.Quantidade < 5)
                .CountAsync();
            
            var estatisticas = new
            {
                TotalProdutos = totalProdutos,
                ValorEstoque = valorEstoque,
                Vencidos = vencidos,
                EstoqueBaixo = estoqueBaixo
            };
            return Ok(estatisticas);
        }

        [HttpGet("Vencidos")]
        public async Task<IActionResult> GetProdutosVencidos()
        {
            var produtosVencidos = await _context.Produto
                .Where(p => p.DataValidade < DateTime.Now)
                .Select(p => new {p.Id, p.Name, p.DataValidade})
                .ToListAsync();
            return Ok(produtosVencidos);
        }

        [HttpGet("Categorias")]
        public async Task<IActionResult> GetDadosCategoria()
        {
            var dados = await _context.Produto
                .Include(p => p.Categoria)
                .GroupBy(p => p.Categoria.Name)
                .Select(c => new 
                {
                   Categoria = c.Key ?? "Sem Categoria",
                     Quantidade = c.Count()
                })
                .ToListAsync();
            return Ok(dados);
        }

        [HttpGet("MovimentacaoDias")]
        public async Task<IActionResult> GetMovimetacaoDias()
        {
            var hoje = DateTime.Now;
            var seteDiasAtras = hoje.AddDays(-6);

            var dados = await _context.Movimentacao
                .Where(m => m.DataMovimentacao.Date >= seteDiasAtras.Date && m.DataMovimentacao.Date <= hoje.Date)
                .ToListAsync();

            var listaEntradas = new List<int>();
            var listaSaidas = new List<int>();
            var listaDias = new List<string>();

            for (int i = 6; i >= 0; i--)
            {
                var diaAnalise = hoje.AddDays(-i).Date;
                listaDias.Add(diaAnalise.ToString("dd/MM"));

                var qtdEntradas = dados
                    .Where(m => m.DataMovimentacao.Date == diaAnalise && m.Tipo == Models.TipoMovimentacao.Entrada)
                    .Sum(m => m.Quantidade);

                var qtdSaidas = dados
                    .Where(m => m.DataMovimentacao.Date == diaAnalise && m.Tipo == Models.TipoMovimentacao.Saida)
                    .Sum(m => m.Quantidade);

                listaEntradas.Add(qtdEntradas);
                listaSaidas.Add(qtdSaidas);
            }

            return Ok(new { dias = listaDias, entradas = listaEntradas, saidas = listaSaidas });
        }
    }
}