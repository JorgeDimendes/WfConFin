using Microsoft.EntityFrameworkCore;
using WfConFin.Models;

namespace WfConFin.Data
{
    public class WfConFinDbContext : DbContext
    {
        public WfConFinDbContext(DbContextOptions<WfConFinDbContext> options) : base(options)
        {
        }

        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Conta> Conta { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}
