using Loggu.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Loggu.Infraestructure.Mapping
{
    public class CheckMapping : IEntityTypeConfiguration<Check>
    {
        public void Configure(EntityTypeBuilder<Check> e)
        {
            e.ToTable(
                "Check",
                tb =>
                {
                    
                    tb.HasCheckConstraint("CK_Check_PneuOk_ZeroUm", "PneuOk IN (0,1)");
                    tb.HasCheckConstraint("CK_Check_FreioOk_ZeroUm", "FreioOk IN (0,1)");
                    tb.HasCheckConstraint("CK_Check_LuzesOk_ZeroUm", "LuzesOk IN (0,1)");
                    tb.HasCheckConstraint("CK_Check_DocumentosOk_ZeroUm", "DocumentosOk IN (0,1)");
                    tb.HasCheckConstraint("CK_Check_AptaParaUso_ZeroUm", "AptaParaUso IN (0,1)");
                }
            );

            e.HasKey(x => x.Id);

            e.Property(x => x.MotoId).IsRequired();
            e.Property(x => x.Quilometragem);

            e.Property(x => x.PneuOk).IsRequired();
            e.Property(x => x.FreioOk).IsRequired();
            e.Property(x => x.LuzesOk).IsRequired();
            e.Property(x => x.DocumentosOk).IsRequired();
            e.Property(x => x.AptaParaUso).IsRequired();

            e.Property(x => x.RealizadoEm).IsRequired();
            e.Property(x => x.Observacao).HasMaxLength(500);
        }
    }
}
