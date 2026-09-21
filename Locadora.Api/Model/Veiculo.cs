using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora.Api.Model;

public class Veiculo
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(8)]
    public string Placa { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Modelo { get; set; } = string.Empty;

    public int AnoFabricacao { get; set; }

    public int Quilometragem { get; set; }

    [ForeignKey(nameof(Fabricante))]
    public int FabricanteId { get; set; }
    public Fabricante Fabricante { get; set; } = null!;

    [ForeignKey(nameof(Categoria))]
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
}