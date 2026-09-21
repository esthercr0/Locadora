using Locadora.Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    public DbSet<Fabricante> Fabricantes => Set<Fabricante>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Aluguel> Alugueis => Set<Aluguel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=LocadoraDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fabricante>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Nome).IsUnique();
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Nome).IsUnique();
        });

        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Cpf).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Veiculo>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Placa).IsUnique();

            e.HasOne(x => x.Fabricante)
             .WithMany(f => f.Veiculos)
             .HasForeignKey(x => x.FabricanteId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Categoria)
             .WithMany(c => c.Veiculos)
             .HasForeignKey(x => x.CategoriaId)
             .OnDelete(DeleteBehavior.Restrict);

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Veiculo_Ano", "[AnoFabricacao] >= 1900");
                t.HasCheckConstraint("CK_Veiculo_Km", "[Quilometragem] >= 0");
            });
        });

        modelBuilder.Entity<Aluguel>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Cliente)
             .WithMany(c => c.Alugueis)
             .HasForeignKey(x => x.ClienteId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Veiculo)
             .WithMany(v => v.Alugueis)
             .HasForeignKey(x => x.VeiculoId)
             .OnDelete(DeleteBehavior.Restrict);

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Aluguel_Periodo", "[DataPrevistaDevolucao] >= [DataRetirada]");
                t.HasCheckConstraint("CK_Aluguel_KmFinal", "[KmFinal] IS NULL OR [KmFinal] >= [KmInicial]");
                t.HasCheckConstraint("CK_Aluguel_ValorDiaria", "[ValorDiaria] > 0");
            });
        });
    }
}
