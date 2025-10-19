using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Loggu.Domain.Entity;

namespace Loggu.Infraestructure.Mapping
{
    public class OcorrenciaMapping : IEntityTypeConfiguration<Ocorrencia>
    {
        public void Configure(EntityTypeBuilder<Ocorrencia> e)
        {
            e.ToTable("Ocorrencia");
            e.HasKey(x => x.Id);

            e.Property(x => x.MotoId)
                .IsRequired();

            e.Property(x => x.Categoria)
                .IsRequired()
                .HasMaxLength(40);

            e.Property(x => x.AcontecidoEm)
                .IsRequired();

            e.Property(x => x.Descricao)
                .HasMaxLength(500);

            e.HasIndex(x => new { x.MotoId, x.AcontecidoEm });
        }
    }
}