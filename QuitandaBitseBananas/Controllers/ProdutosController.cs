using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using QuitandaBitseBananas.Data;
using QuitandaBitseBananas.Models;

namespace QuitandaBitseBananas.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly QuitandaBitseBananasContext _context;

        public ProdutosController(QuitandaBitseBananasContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string termoPesquisa)
        {
            var consulta = _context.Produto
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .AsQueryable(); // Permite construir consultas dinamicamente

            if (!string.IsNullOrEmpty(termoPesquisa))
            {
                consulta = consulta.Where(p => p.Name.Contains(termoPesquisa));
            }

            ViewData["PesquisaAtual"] = termoPesquisa;

            consulta = consulta.OrderBy(p => p.DataValidade);
            return View(await consulta.ToListAsync());

            var produtos = _context.Produto
                .Include(produtos => produtos.Categoria)
                .Include(produtos => produtos.Fornecedor);

            return View(await produtos.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.UnidadesMedida = Enum.GetValues(typeof(UnidadeMedida))
                .Cast<UnidadeMedida>()
                .Select(u => new SelectListItem
                {
                    Value = ((int)u).ToString(),
                    Text = u.GetDisplayName()
                }).ToList();

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Name");
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Preco,Unidade,Quantidade,DataEntrada,DataValidade,CategoriaId,FornecedorId")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                // Calculando o valor total (Preço x Quantidade)
                produto.ValorTotal = produto.Preco * produto.Quantidade;

                if (produto.DataEntrada == DateTime.MinValue)
                {
                    produto.DataEntrada = DateTime.Now;

                }

                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Recarregar os dados para os dropdowns em caso de erro
            ViewBag.UnidadesMedida = Enum.GetValues(typeof(UnidadeMedida))
                .Cast<UnidadeMedida>()
                .Select(u => new SelectListItem
                {
                    Value = ((int)u).ToString(),
                    Text = u.GetDisplayName()
                }).ToList();
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Name", produto.CategoriaId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Name", produto.FornecedorId);

            return View(produto);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            // Carregar as listas para a tela de edição
        ViewBag.UnidadesMedida = Enum.GetValues(typeof(UnidadeMedida))
        .Cast<UnidadeMedida>()
        .Select(u => new SelectListItem
        {
            Value = ((int)u).ToString(),
            Text = u.GetDisplayName(),
            Selected = u == produto.Unidade // Já deixa a unidade certa selecionada
        }).ToList();

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Name", produto.CategoriaId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Name", produto.FornecedorId);

            return View(produto);
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Preco,Unidade,Quantidade,DataEntrada,DataValidade,CategoriaId,FornecedorId")] Produto produto)
        {
            if (id != produto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculando o valor total (Preço x Quantidade)
                    produto.ValorTotal = produto.Preco * produto.Quantidade;
                    
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id)) return NotFound();
                    
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            
            // Se falhar, recarrega
            ViewBag.UnidadesMedida = Enum.GetValues(typeof(UnidadeMedida))
                .Cast<UnidadeMedida>()
                .Select(u => new SelectListItem
                {
                    Value = ((int)u).ToString(),
                    Text = u.GetDisplayName()
                }).ToList();

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Name", produto.CategoriaId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Name", produto.FornecedorId);
            
            return View(produto);
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);

        }

        [HttpPost]
        [Route("Produtos/ApagarRapido")]
        public async Task<IActionResult> ApagarRapido([FromBody] int id)
        {
            // Procura o produto no banco pelo ID
            var produto = await _context.Produto.FindAsync(id);
            
            if (produto == null) return NotFound("Produto não encontrado");

            // Remove e salva as mudanças            
            _context.Remove(produto);
             await _context.SaveChangesAsync();

            // Retorna sucesso (Syntaxe corrigida: 'new' em vez de 'Delete')
            return Json(new { success = true, message = "Produto apagado com sucesso!" });
        }
    }
}
    
    // Classe de Extensão para obter o nome legível do Enum
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                                             .GetMember(enumValue.ToString())
                                             .FirstOrDefault()
                                             ?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.GetName() ?? enumValue.ToString(); // Retorna o nome display ou o nome do enum
        }
    }
