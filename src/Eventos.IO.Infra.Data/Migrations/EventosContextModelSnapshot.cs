﻿// <auto-generated />
using System;
using Eventos.IO.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eventos.IO.Infra.Data.Migrations
{
    [DbContext(typeof(EventosContext))]
    partial class EventosContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Eventos.IO.Domain.EventosRoot.Categoria", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("Eventos.IO.Domain.EventosRoot.Endereco", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("CEP")
                        .IsRequired()
                        .HasColumnType("varchar(8)")
                        .HasMaxLength(8);

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Complemento")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<Guid?>("EventoId");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Numero")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("EventoId")
                        .IsUnique()
                        .HasFilter("[EventoId] IS NOT NULL");

                    b.ToTable("Enderecos");
                });

            modelBuilder.Entity("Eventos.IO.Domain.EventosRoot.Evento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CategoriaId");

                    b.Property<DateTime>("DataFim");

                    b.Property<DateTime>("DataInicio");

                    b.Property<string>("DescricaoCurta")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("DescricaoLonga")
                        .HasColumnType("varchar(max)");

                    b.Property<Guid?>("EnderecoId");

                    b.Property<bool>("Excluido");

                    b.Property<bool>("Gratuito");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(150)");

                    b.Property<string>("NomeEmpresa")
                        .IsRequired()
                        .HasColumnType("varchar(150)");

                    b.Property<bool>("Online");

                    b.Property<Guid>("OrganizadorId");

                    b.Property<decimal>("Valor");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("OrganizadorId");

                    b.ToTable("Eventos");
                });

            modelBuilder.Entity("Eventos.IO.Domain.OrganizadoresRoot.Organizador", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasColumnType("varchar(11)")
                        .HasMaxLength(11);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Organizadores");
                });

            modelBuilder.Entity("Eventos.IO.Domain.EventosRoot.Endereco", b =>
                {
                    b.HasOne("Eventos.IO.Domain.EventosRoot.Evento", "Evento")
                        .WithOne("Endereco")
                        .HasForeignKey("Eventos.IO.Domain.EventosRoot.Endereco", "EventoId");
                });

            modelBuilder.Entity("Eventos.IO.Domain.EventosRoot.Evento", b =>
                {
                    b.HasOne("Eventos.IO.Domain.EventosRoot.Categoria", "Categoria")
                        .WithMany("Eventos")
                        .HasForeignKey("CategoriaId");

                    b.HasOne("Eventos.IO.Domain.OrganizadoresRoot.Organizador", "Organizador")
                        .WithMany("Eventos")
                        .HasForeignKey("OrganizadorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
