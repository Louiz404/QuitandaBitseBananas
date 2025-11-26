using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuitandaBitseBananas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace QuitandaBitseBananas.Data
{
    public class QuitandaBitseBananasContext : IdentityDbContext
    {
        public QuitandaBitseBananasContext (DbContextOptions<QuitandaBitseBananasContext> options)
            : base(options)
        {
        }

        public DbSet<QuitandaBitseBananas.Models.Produto> Produto { get; set; } = default!;
        public DbSet<QuitandaBitseBananas.Models.Fornecedor> Fornecedor { get; set; }
        public DbSet<QuitandaBitseBananas.Models.Categoria> Categoria { get; set; }
        public DbSet<QuitandaBitseBananas.Models.Movimentacao> Movimentacao { get; set; }
    }
}
