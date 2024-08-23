using Domain.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.configurations.entitymaps
{
    public class ConfiguracaoTarefa : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.HasKey(x=> x.Id);
            builder.Property(p => p.Id)
                            .HasColumnType("int")
                            .UseIdentityColumn();
            builder.Property(p => p.Descricao)
                            .HasColumnType("nvarchar(500)")
                            .IsRequired(true);
            builder.Property(p => p.Status)
                           .HasColumnType("char(1)")
                           .IsRequired(true);
            builder.Property(p => p.DataTarefa)
                           .HasColumnType("datetime")
                           .IsRequired(true);
        }
    }
}
