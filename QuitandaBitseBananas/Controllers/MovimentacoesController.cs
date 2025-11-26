using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuitandaBitseBananas.Data;
using QuitandaBitseBananas.Models;
using System;

namespace QuitandaBitseBananas.Controllers
{
    [Authorize]
    public class MovimentacoesController : Controller
    {
        private readonly QuitandaBitseBananasContext _context;

        public MovimentacoesController(QuitandaBitseBananasContext context)
        {
            _context = context;
        }

        // GET: Movimentacoes/HistoricoGeral
        [HttpGet]
        [Route("Movimentacoes/HistoricoGeral")]
        public async Task<IActionResult> HistoricoGeral()
        {
            var movimentacoes = await _context.Movimentacao
                .Include(m => m.Produto) // Importante: Traz o nome do produto
                .OrderByDescending(m => m.DataMovimentacao) // Do mais recente pro mais antigo
                .Take(100) // Pega só as últimas 100 movimentações
                .ToListAsync();

            return View(movimentacoes);
        }

        [HttpPost]
        [Route("Movimentacoes/Lancar")]
        public async Task<IActionResult> Lancar([FromBody] DadosMovimentacao dados)
        {
            var produto = await _context.Produto.FindAsync(dados.ProdutoId);

            if (produto == null) return BadRequest("Produto não encontrado.");

            var novaMovimentacao = new Movimentacao
            {
                ProdutoId = dados.ProdutoId,
                DataMovimentacao = DateTime.Now,
                Quantidade = dados.Quantidade,
                Tipo = dados.Tipo,
                Motivo = dados.Motivo,
                Observacoes = dados.Observacao,
                UsuarioId =User.Identity.Name ?? "Sistema" // Dedo duro
            };
            if (dados.Tipo == TipoMovimentacao.Entrada)
            {
                produto.Quantidade += dados.Quantidade;
            }

            else
            {
                if (produto.Quantidade < dados.Quantidade)
                {
                    return BadRequest($"Quantidade insuficiente em estoque para saída. Você tem {produto.Quantidade} mas tentou tirar {dados.Quantidade}");
                }
                produto.Quantidade -= dados.Quantidade;
            }

            produto.ValorTotal = produto.Preco * produto.Quantidade;

            _context.Add(novaMovimentacao);
            _context.Update(produto);
            await _context.SaveChangesAsync();

            return Json(new { success = true, novaQuantidade = produto.Quantidade, mensagem = "Movimentação lançada com sucesso!" });

        }

        [HttpGet]
        [Route("Movimentacoes/ObterPorProduto/{produtoId}")]
        public async Task<IActionResult> Historico(int produtoId)
        {
           var movimentacoes = await _context.Movimentacao
           .Include(m => m.Produto)
           .Where(m => m.ProdutoId == produtoId)
           .OrderByDescending(m => m.DataMovimentacao)
           .ToListAsync();

            if (!movimentacoes.Any())
            {
                var produto = await _context.Produto.FindAsync(produtoId);
                if (produto == null) return NotFound("Produto não encontrado.");
                ViewBag.ProdutoNome = produto.Name;
            }
            else
            {
                ViewBag.NomeProduto = movimentacoes.First().Produto.Name;
            }
            return View(movimentacoes);
        }

    }

    // Classe auxiliar para receber os dados do JavaScript
    public class DadosMovimentacao
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public MotivoMovimentacao Motivo { get; set; }
        public string? Observacao { get; set; }
    }
}
