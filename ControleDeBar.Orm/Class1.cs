using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Orm.ModuloMesa;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Orm
{
    public class ControleDeBarDbContext : DbContext
    {
        public DbSet<Mesa> Mesas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MapeadorMesaemOrm());
            base.OnModelCreating(modelBuilder);
        }
    }
}
