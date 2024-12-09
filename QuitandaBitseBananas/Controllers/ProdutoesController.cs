using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuitandaBitseBananas.Data;
using QuitandaBitseBananas.Models;

namespace QuitandaBitseBananas.Controllers
{
    public class ProdutoesController : Controller
    {
        private readonly QuitandaBitseBananasContext _context;

        public ProdutoesController(QuitandaBitseBananasContext context)
        {
            _context = context;
        }

        // GET: Produtoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produto.ToListAsync());
        }

        // GET: Produtoes/Details/5
        public async Task<IActionResult> Details(long? id)
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

        // GET: Produtoes/Create
        public IActionResult Create()
        {
            // Adiciona unidades de medida na view
            ViewBag.UnidadesMedida = Enum.GetValues(typeof(UnidadeMedida))
                                          .Cast<UnidadeMedida>()
                                          .Select(u => new SelectListItem
                                          {
                                              Value = ((int)u).ToString(),
                                              Text = u.GetDisplayName()
                                          }).ToList();

            return View();
        }

        // POST: Produtoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Preco,Unidade,Quantidade")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                // Calculando o valor total (Preço x Quantidade)
                produto.ValorTotal = produto.Preco * produto.Quantidade;

                // Adiciona o produto com o valor total calculado
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produtoes/Edit/5
        public async Task<IActionResult> Edit(long? id)
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

            // Adiciona unidades de medida na view
            ViewBag.UnidadesMedida = new SelectList(
                Enum.GetValues(typeof(UnidadeMedida))
                    .Cast<UnidadeMedida>()
                    .Select(u => new SelectListItem
                    {
                        Value = ((int)u).ToString(),
                        Text = u.GetDisplayName() // Usa a extensão para pegar o nome legível
                    }),
                "Value", "Text", produto.Unidade); // Passa a unidade do produto para o select

            return View(produto);
        }

        // POST: Produtoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Preco,Unidade,Quantidade,ValorTotal")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound(); // Se o id do produto não bater com o parâmetro id, retorna NotFound
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculando o valor total (Preço x Quantidade)
                    produto.ValorTotal = produto.Preco * produto.Quantidade;

                    // Atualiza o produto com o novo valor total
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound(); // Se o produto não existir no banco, retorna NotFound
                    }
                    else
                    {
                        throw; // Caso contrário, relança a exceção
                    }
                }
                return RedirectToAction(nameof(Index)); // Após salvar, redireciona para o índice (lista de produtos)
            }

            // Caso a validação falhe, repassa as unidades de medida e os dados do produto para a view
            ViewBag.UnidadesMedida = new SelectList(
                Enum.GetValues(typeof(UnidadeMedida))
                    .Cast<UnidadeMedida>()
                    .Select(u => new SelectListItem
                    {
                        Value = ((int)u).ToString(),
                        Text = u.GetDisplayName()
                    }),
                "Value", "Text", produto.Unidade); // Mantém a unidade selecionada

            return View(produto); // Retorna a view de edição com os dados do produto
        }

        // GET: Produtoes/Delete/5
        public async Task<IActionResult> Delete(long? id)
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

        // POST: Produtoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var produto = await _context.Produto.FindAsync(id);
            if (produto != null)
            {
                _context.Produto.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Após excluir, redireciona para o índice
        }

        private bool ProdutoExists(long id)
        {
            return _context.Produto.Any(e => e.Id == id); // Verifica se o produto existe no banco
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
}
