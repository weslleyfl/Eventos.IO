using Eventos.IO.Domain.OrganizadoresRoot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventos.IO.Infra.Data.Mappings
{
    public class OrganizadorMapping : IEntityTypeConfiguration<Organizador>
    {
        public void Configure(EntityTypeBuilder<Organizador> builder)
        {
            builder.Property(e => e.Nome)
               .HasColumnType("varchar(150)")
               .IsRequired();

            builder.Property(e => e.Email)
               .HasColumnType("varchar(100)")
               .IsRequired();

            builder.Property(e => e.CPF)
               .HasColumnType("varchar(11)")
               .HasMaxLength(11)
               .IsRequired();

            builder.Ignore(e => e.ValidationResult);

            builder.Ignore(e => e.CascadeMode);

            builder.ToTable("Organizadores");
        }

     
    }
}