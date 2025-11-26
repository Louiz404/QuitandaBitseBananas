using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuitandaBitseBananas.Data;
using QuitandaBitseBananas.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuitandaBitseBananas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuitandaBitseBananasContext _context;

        public HomeController(ILogger<HomeController> logger, QuitandaBitseBananasContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Estatisticas para o dashboard
            ViewBag.TotalProdutos = await _context.Produto.CountAsync();

            ViewBag.ValorEstoque = await _context.Produto.SumAsync(p => p.ValorTotal);

            ViewBag.Vencidos =await _context.Produto
                .Where(p => p.DataValidade < DateTime.Now)
                .CountAsync();

            ViewBag.EstoqueBaixo = await _context.Produto
                .Where(p => p.Quantidade < 5)
                .CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
