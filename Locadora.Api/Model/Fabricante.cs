using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Model;

public class Fabricante
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(60)]
    public string? PaisOrigem { get; set; }

    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}