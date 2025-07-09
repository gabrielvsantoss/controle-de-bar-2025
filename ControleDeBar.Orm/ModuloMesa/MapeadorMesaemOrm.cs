
using ControleDeBar.Dominio.ModuloMesa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Orm.ModuloMesa
{
    public class MapeadorMesaemOrm : IEntityTypeConfiguration<Mesa>
    {
        public void Configure(EntityTypeBuilder<Mesa> builder)
        {
            builder.Property(x => x.Id)
                . ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.Numero)
                .IsRequired();

            builder.Property(x => x.Capacidade)
               .IsRequired();

            builder.Property(x => x.EstaOcupada)
               .IsRequired();
        }
    }
    

    
}
