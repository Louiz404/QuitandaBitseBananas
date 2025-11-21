using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuitandaBitseBananas.Models;

namespace QuitandaBitseBananas.Data
{
    public class QuitandaBitseBananasContext : DbContext
    {
        public QuitandaBitseBananasContext (DbContextOptions<QuitandaBitseBananasContext> options)
            : base(options)
        {
        }

        public DbSet<QuitandaBitseBananas.Models.Produto> Produto { get; set; } = default!;
        public DbSet<QuitandaBitseBananas.Models.Fornecedor> Fornecedor { get; set; }
        public DbSet<QuitandaBitseBananas.Models.Categoria> Categoria { get; set; }
    }
}
