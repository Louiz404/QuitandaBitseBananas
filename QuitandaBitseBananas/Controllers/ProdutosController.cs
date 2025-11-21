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
        private readonly IWebHostEnvironment hostEnvironment;

        public ProdutosController(QuitandaBitseBananasContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Preco,Unidade,Quantidade,DataEntrada,DataValidade,CategoriaId,FornecedorId")] Produto produto, IFormFile arquivoImagem)
        {
            if (ModelState.IsValid)
            {
                // Calculando o valor total (Preço x Quantidade)
                produto.ValorTotal = produto.Preco * produto.Quantidade;

                if (produto.DataEntrada == DateTime.MinValue)
                {
                    produto.DataEntrada = DateTime.Now;

                }
                if (arquivoImagem != null)
                {
                    string wwwRootPath = hostEnvironment.WebRootPath;
                    string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivoImagem.FileName);
                    string caminhoArquivo = Path.Combine(wwwRootPath, "imagens");

                    if (!Directory.Exists(caminhoArquivo))
                    {
                        Directory.CreateDirectory(caminhoArquivo);
                    }

                    string caminhoCompleto = Path.Combine(caminhoArquivo, nomeArquivo);
                    using (var fileStream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await arquivoImagem.CopyToAsync(fileStream);
                    }
                    produto.Imagem = nomeArquivo;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Preco,Unidade,Quantidade,DataEntrada,DataValidade,CategoriaId,FornecedorId,Imagem")] Produto produto, IFormFile arquivoImagem, bool removerImagem)
        {
            if (id != produto.Id) return NotFound();
            ModelState.Remove("arquivoImagem");
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. BUSCAR O PRODUTO ORIGINAL NO BANCO (Para saber qual era a imagem antiga)
                    // Usamos AsNoTracking para não travar o Entity Framework, pois vamos atualizar o 'produto' que veio do form
                    var produtoOriginal = await _context.Produto.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Caminho base das imagens
                    string caminhoPasta = Path.Combine(hostEnvironment.WebRootPath, "imagens");
                    
                    // CENÁRIO A: REMOVER A FOTO (Checkbox marcado)
                    if (removerImagem)
                    {
                        if (!string.IsNullOrEmpty(produtoOriginal.Imagem))
                        {
                            string caminhoImagemAntiga = Path.Combine(caminhoPasta, produtoOriginal.Imagem);
                            if (System.IO.File.Exists(caminhoImagemAntiga))
                            {
                                System.IO.File.Delete(caminhoImagemAntiga);
                            }
                            
                            // Zera o campo no banco
                            produto.Imagem = null;
                        }
                    }

                    // CENÁRIO B: TROCAR A FOTO (Novo arquivo enviado)
                    else if (arquivoImagem != null)
                    {
                        if (!string.IsNullOrEmpty(produtoOriginal.Imagem))
                        {
                            string caminhoImagemAntiga = Path.Combine(caminhoPasta, produtoOriginal.Imagem);
                            if (System.IO.File.Exists(caminhoImagemAntiga))
                            {
                                System.IO.File.Delete(caminhoImagemAntiga);
                            }
                        }
                        string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivoImagem.FileName);
                        string caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);
                        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                        {
                            await arquivoImagem.CopyToAsync(stream);
                        }
                        produto.Imagem = nomeArquivo;

                    }
                    
                    // CENÁRIO C: MANTER A ANTIGA (Nenhum arquivo novo e checkbox desmarcado)
                    else 
                    {
                    produto.Imagem = produtoOriginal.Imagem;
                    }

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
