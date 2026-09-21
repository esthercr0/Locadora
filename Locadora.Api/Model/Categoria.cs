using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora.Api.Model;

public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(60)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Descricao { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorDiariaBase { get; set; }

    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}